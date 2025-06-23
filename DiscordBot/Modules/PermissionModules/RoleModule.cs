using Discord.Commands;

namespace DiscordBot.Modules.PermissionModules;

[Discord.Interactions.Group("role", "role commands - group.")]

public class RoleModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 指定したユーザーに指定したロールを付与するコマンド
    // </summary>
    [SlashCommand("add", "指定したユーザーに指定したロールを付与します。(権限必要)")]
    [Discord.Interactions.RequireUserPermission(GuildPermission.ManageRoles)]
    [Discord.Interactions.RequireBotPermission(GuildPermission.ManageRoles)]

    public async Task RoleAddCommandAsync([Discord.Interactions.Summary(description: "指定するユーザーを選択してください。")] SocketGuildUser targetUser, [Remainder] IRole roleId)
    {
        await targetUser.AddRoleAsync(roleId);

        var embedBuilder = new EmbedBuilder()
            .WithTitle("ロールを付与しました:mailbox_with_mail:")
            .WithDescription($"対象者: {targetUser.Mention}\n" +
                             $"ロール: {roleId.Mention}")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }

    // <summary>
    // 指定したユーザーから指定したロールを剥奪するコマンド
    // </summary>
    [SlashCommand("remove", "指定したユーザーから指定したロールを剥奪します。(権限必要)")]
    [Discord.Interactions.RequireUserPermission(GuildPermission.ManageRoles)]
    [Discord.Interactions.RequireBotPermission(GuildPermission.ManageRoles)]
    public async Task RoleRemoveCommandAsync([Discord.Interactions.Summary(description: "指定するユーザーを選択してください。")] SocketGuildUser targetUser, [Remainder] IRole roleId)
    {
        await targetUser.RemoveRoleAsync(roleId);

        var embedBuilder = new EmbedBuilder()
            .WithTitle("ロールを剥奪しました:mailbox_with_no_mail:")
            .WithDescription($"対象者: {targetUser.Mention}\n" +
                             $"ロール: {roleId.Mention}")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }
}
