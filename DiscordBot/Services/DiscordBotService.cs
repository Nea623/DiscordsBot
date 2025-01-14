using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32.SafeHandles;

namespace DiscordBot.Services;

public class DiscordBotService(DiscordSocketClient client, InteractionService interactions, InteractionHandler interactionHandler,
    IConfiguration configuration, ILogger<DiscordBotService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        client.Log += Log;
        interactions.Log += Log;

        client.Ready += Ready;
        client.UserJoined += OnUserJoined;

        await interactionHandler.InitializeAsync();
        await client.LoginAsync(TokenType.Bot, configuration["DiscordBot:Token"]);
        await client.StartAsync();
        //Botのステータス
        await client.SetStatusAsync(UserStatus.AFK);
        //カスタムステータス
        await client.SetGameAsync("テスト中...");

        await Task.Delay(-1, cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        if (ExecuteTask == null)
            return Task.CompletedTask;

        base.StopAsync(cancellationToken);
        return client.StopAsync();
    }

    public Task Log(LogMessage message)
    {
        var severity = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information
        };

        logger.Log(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        return Task.CompletedTask;
    }

    public Task Ready()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                // ApplicationCommands の登録をここで行う。
                await interactions.RegisterCommandsToGuildAsync(ulong.Parse(configuration["DiscordBot:GuildId"] ?? ""));
                await interactions.RegisterCommandsToGuildAsync(ulong.Parse(configuration["DiscordBot:GuildId2"] ?? ""));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Message}", ex.Message);
            }
        });

        return Task.CompletedTask;
    }

    //新規ユーザーがサーバーに入室したときにコメントするやつ。
    private Task OnUserJoined(SocketGuildUser user)
    {
        _ = Task.Run(async () =>
        {
            SocketGuild guild = user.Guild;
            string avatar = user.GetAvatarUrl();

            var embedBuilder = new EmbedBuilder()
                .WithTitle("新規ユーザーが入室しました！")
                .WithDescription($"{user.Mention}さん、**{user.Guild.Name}**へようこそ！\n" +
                                 $"あなたは{guild.MemberCount - guild.Users.Count(x => x.IsBot)}人目のメンバーです。\n" +
                                 $"新規さんを歓迎しよう🎉")
                .WithThumbnailUrl(avatar)
                .WithColor(0x8DCE3E);

            await (user.Guild.SystemChannel).SendMessageAsync(embed: embedBuilder.Build());
        });

        return Task.CompletedTask;
    }
}
