using Discord.Commands;

namespace DiscordBot.Modules.PermissionModules;

public class invitelistModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 招待一覧を取得するコマンド
    // </summary>。
    [SlashCommand("invitelist", "招待の一覧を表示します。")]
    [Discord.Commands.RequireUserPermission(GuildPermission.ManageGuild)]
    [Discord.Commands.RequireBotPermission(GuildPermission.ManageGuild)]
    public async Task GetInvitesAsync()
    {
        var guild = Context.Guild;

        // 招待一覧を取得
        var invites = await guild.GetInvitesAsync();
        var invite = invites.FirstOrDefault();

        if (!invites.Any())
        {
            await ReplyAsync("このサーバーには招待リンクがありません。");
            return;
        }

        var inviteList = invites.ToList();
        var embedBuilder = new EmbedBuilder()
            .WithTitle(":clipboard: 招待の一覧");

        for (int i = 0; i < inviteList.Count; i++)
        {
            var inv = inviteList[i];
            embedBuilder.AddField(
                $":link: 招待コード: `{inv.Code}`",
                $"招待者: **{inv.Inviter.Username}**( {inv.Inviter.Mention} ) / 使用回数: **{inv.Uses}** / 有効期限: {(inv.MaxAge == null || inv.MaxAge == 0 ? "**無制限**" : $"**{inv.MaxAge / 60}分**")}"
            );
        }

        embedBuilder.WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl());
        embedBuilder.WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }
}