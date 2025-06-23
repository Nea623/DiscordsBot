using Discord.Interactions;

namespace DiscordBot.Modules.PermissionModules;

public class invitesModule : InteractionModuleBase<SocketInteractionContext>
{
    /// <summary>
    /// 招待一覧を取得、または特定の招待コードを表示するコマンド
    /// </summary>
    [SlashCommand("invites", "招待の一覧、または指定した招待コードの詳細を表示します。")]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    [RequireBotPermission(GuildPermission.ManageGuild)]
    public async Task GetInvitesAsync([Summary(description: "詳細を確認したい招待コードを入力してください。")] string? code = null)
    {
        var guild = Context.Guild;
        var invites = await guild.GetInvitesAsync();

        if (!invites.Any())
        {
            await RespondAsync("このサーバーには招待リンクがありません。");
            return;
        }

        var embedBuilder = new EmbedBuilder().WithColor(0x8DCE3E);

        if (!string.IsNullOrWhiteSpace(code))
        {
            // 招待コードで検索
            var found = invites.FirstOrDefault(inv => inv.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            if (found == null)
            {
                await RespondAsync($"指定された招待コード `{code}` は見つかりませんでした。");
                return;
            }

            embedBuilder.WithTitle(":mag: 招待コードの詳細");
            embedBuilder.AddField(
                $":link: 招待コード: `{found.Code}`",
                $"招待者: **{found.Inviter.Username}** ({found.Inviter.Mention})\n" +
                $"使用回数: **{found.Uses}**\n" +
                $"有効期限: {(found.MaxAge == null || found.MaxAge == 0 ? "**無制限**" : $"**{found.MaxAge / 60}分**")}"
            );
        }
        else
        {
            embedBuilder.WithTitle(":clipboard: 招待の一覧");

            foreach (var inv in invites)
            {
                embedBuilder.AddField(
                    $":link: 招待コード: `{inv.Code}`",
                    $"招待者: **{inv.Inviter.Username}** ({inv.Inviter.Mention}) / 使用回数: **{inv.Uses}** / 有効期限: {(inv.MaxAge == null || inv.MaxAge == 0 ? "**無制限**" : $"**{inv.MaxAge / 60}分**")}"
                );
            }
        }

        embedBuilder.WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl());

        await RespondAsync(embed: embedBuilder.Build());
    }
}