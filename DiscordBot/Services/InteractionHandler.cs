﻿using System.Reflection;
using DiscordBot.Modules.OtherModules;

namespace DiscordBot.Services;

public class InteractionHandler(DiscordSocketClient client, InteractionService interactions, IServiceProvider services, ILogger<InteractionHandler> logger)
{
    /// <summary>
    ///     InteractionService の初期化
    /// </summary>
    public async Task InitializeAsync()
    {
        await interactions.AddModulesAsync(Assembly.GetEntryAssembly(), services);

        client.InteractionCreated += OnInteractionCreated;
        interactions.InteractionExecuted += OnInteractionExecuted;
    }

    private async Task OnInteractionCreated(SocketInteraction interaction)
    {
        try
        {
            // ボタン押下などのインタラクションもここに来る
            if (interaction is SocketMessageComponent component &&
                component.Data.CustomId.StartsWith("roulette_retry:"))
            {
                // 手動でモジュールを呼び出して処理する（依存性注入が必要なら書き換えてください）
                var module = new RouletteModule();  // ← 実際にはDIで生成するのが理想
                await module.HandleComponent(component);
                return;
            }

            var context = new SocketInteractionContext(client, interaction);
            var result = await interactions.ExecuteCommandAsync(context, services);

            if (!result.IsSuccess)
                _ = Task.Run(() => HandleInteractionExecutedResult(context.Interaction, result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Message}", ex.Message);
        }
    }

    private Task OnInteractionExecuted(ICommandInfo command, IInteractionContext context, IResult result)
    {
        var userName = context.User?.GlobalName ?? context.User?.Username ?? "不明";
        var userId = context.User?.Id.ToString() ?? "不明";

        var moduleName = command?.Module?.Name ?? "不明";
        var methodName = command?.MethodName ?? "不明";

        var guildId = context.Guild?.Id.ToString() ?? "DM";
        var channelId = context.Channel?.Id.ToString() ?? "不明";

        logger.LogInformation(
            "[コマンド実行] {UserName}({UserId}), command={ModuleName}.{MethodName}, guild_id={GuildId}, channel_id={ChannelId}",
            userName, userId, moduleName, methodName, guildId, channelId
        );

        if (!result.IsSuccess)
        {
            _ = Task.Run(() => HandleInteractionExecutedResult(context.Interaction, result));
        }

        return Task.CompletedTask;
    }

    private async Task HandleInteractionExecutedResult(IDiscordInteraction interaction, IResult result)
    {
        switch (result.Error)
        {
            case InteractionCommandError.UnmetPrecondition:
                logger.LogInformation("[コマンド実行] UnmetPrecondition - {ErrorReason}, user_id={UserId}, guild_id={GuildId}, channel_id={ChannelId}", result.ErrorReason, interaction.User.Id, interaction.GuildId ?? 0, interaction.ChannelId);
                break;
            case InteractionCommandError.BadArgs:
                logger.LogInformation("[コマンド実行] BadArgs - {ErrorReason}, user_id={UserId}, guild_id={GuildId}, channel_id={ChannelId}", result.ErrorReason, interaction.User.Id, interaction.GuildId ?? 0, interaction.ChannelId);
                break;
            case InteractionCommandError.ConvertFailed:
                logger.LogInformation("[コマンド実行] ConvertFailed - {ErrorReason}, user_id={UserId}, guild_id={GuildId}, channel_id={ChannelId}", result.ErrorReason, interaction.User.Id, interaction.GuildId ?? 0, interaction.ChannelId);
                break;
            case InteractionCommandError.Exception:
                logger.LogInformation("[コマンド実行] Exception - {ErrorReason}, user_id={UserId}, guild_id={GuildId}, channel_id={ChannelId}", result.ErrorReason, interaction.User.Id, interaction.GuildId ?? 0, interaction.ChannelId);
                break;
            case InteractionCommandError.ParseFailed:
                logger.LogInformation("[コマンド実行] ParseFailed - {ErrorReason}, user_id={UserId}, guild_id={GuildId}, channel_id={ChannelId}", result.ErrorReason, interaction.User.Id, interaction.GuildId ?? 0, interaction.ChannelId);
                break;
            case InteractionCommandError.Unsuccessful:
                logger.LogInformation("[コマンド実行] Unsuccessful - {ErrorReason}, user_id={UserId}, guild_id={GuildId}, channel_id={ChannelId}", result.ErrorReason, interaction.User.Id, interaction.GuildId ?? 0, interaction.ChannelId);
                break;
        }

        if (!interaction.HasResponded)
        {
            await interaction.RespondAsync($"失敗: {result.ErrorReason}", ephemeral: true);
        }
        else
        {
            await interaction.FollowupAsync($"失敗: {result.ErrorReason}", ephemeral: true);
        }
    }
}
