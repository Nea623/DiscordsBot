using Discord;
using Discord.Commands;

namespace DiscordBot.Modules.PermissionModules;

public class BanModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 指定したユーザーをBANするコマンド
    // </summary>
    [SlashCommand("ban", "指定したユーザーをBANします。(権限必要)")]
    [Discord.Commands.RequireUserPermission(GuildPermission.BanMembers)]
    [Discord.Commands.RequireBotPermission(GuildPermission.BanMembers)]
    public async Task BanCommandAsync([Discord.Interactions.Summary(description: "指定するユーザーを選択してください。")] SocketGuildUser targetUser, [Discord.Interactions.Summary(description: "理由を入力してください。")][Remainder] string reason = null)
    {
        if (reason == null)
        {
            reason = "理由はありません";
        }
        await Context.Guild.AddBanAsync(targetUser.Id, 0, reason);

        var embedBuilder = new EmbedBuilder()
            .WithTitle($"{targetUser.GlobalName ?? targetUser.Username}はBANされました:wave:")
            .WithDescription($"理由: {reason}")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }

    // <summary>
    // 指定したユーザーのBANを解除するコマンド
    // </summary>
    [SlashCommand("unban", "指定したユーザーのBANを解除します。(権限必要)")]
    [Discord.Commands.RequireUserPermission(GuildPermission.BanMembers)]
    [Discord.Commands.RequireBotPermission(GuildPermission.BanMembers)]
    public async Task UnBanCommandAsync([Discord.Interactions.Summary(description: "指定するユーザーを選択してください。")] SocketUser targetUser)
    {
        await Context.Guild.RemoveBanAsync(targetUser);

        var embedBuilder = new EmbedBuilder()
            .WithTitle($"{targetUser.GlobalName ?? targetUser.Username}が解除されました:clap:")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }
}
