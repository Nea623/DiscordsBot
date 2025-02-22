using Discord.Commands;

namespace DiscordBot.Modules.PermissionModules;

public class TimeoutModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 指定したユーザーを指定した時間タイムアウトするコマンド
    // </summary>
    [SlashCommand("timeout", "指定したユーザーを指定した時間タイムアウトします。(権限必要)")]
    [Discord.Commands.RequireBotPermission(GuildPermission.ModerateMembers)]
    [Discord.Commands.RequireUserPermission(GuildPermission.ModerateMembers)]
    public async Task TimeoutCommandAsync([Discord.Interactions.Summary(description: "指定するユーザーを選択してください。")] SocketGuildUser targetUser, [Discord.Interactions.Summary(description: "// 時間設定はm, h, dで設定。(minutes(分)hours(時)day(日))")] [Remainder] TimeSpan span)
    {
        await targetUser.SetTimeOutAsync(span); // 時間設定はm, h, dがある。(minutes(分)hours(時)day(日))

        var embedBuilder = new EmbedBuilder()
            .WithTitle("タイムアウトされました:zipper_mouth:")
            .WithDescription($"対象者: {targetUser.Mention}\n" +
                             $"設定時間: {span}")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }

    // <summary>
    // 指定したユーザーのタイムアウトを解除するコマンド
    // </summary>
    [SlashCommand("untimeout", "指定したユーザーのタイムアウトを解除します。(権限必要)")]
    [Discord.Commands.RequireBotPermission(GuildPermission.ModerateMembers)]
    [Discord.Commands.RequireUserPermission(GuildPermission.ModerateMembers)]
    public async Task RoleRemoveCommandAsync([Discord.Interactions.Summary(description: "指定するユーザーを選択してください。")] SocketGuildUser targetUser)
    {
        await targetUser.RemoveTimeOutAsync();

        var embedBuilder = new EmbedBuilder()
            .WithTitle("タイムアウトを解除しました:grinning:")
            .WithDescription($"対象者: {targetUser.Mention}")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }
}
