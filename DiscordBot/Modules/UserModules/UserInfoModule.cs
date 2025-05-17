using Discord.Commands;

namespace DiscordBot.Modules.UserModules;

public class UserInfoModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 指定されたユーザーの情報を表示するコマンド
    // </summary>
    [SlashCommand("userinfo", "指定されたユーザーの情報を表示します。")]
    public async Task UserInfoCommandAsync([Discord.Interactions.Summary(description: "指定するユーザーを選択してください。")] [Remainder] IGuildUser user = null)
    {
        if (user is null) user = (IGuildUser)Context.User; // ユーザーが指定されていない場合は実行者の情報を表示する

        string avatar = user.GetAvatarUrl();
        string status = null;
        string isbot;
        string nickname;
        string platformInfo = null;

        // キャストして SocketGuildUser にする
        var socketUser = user as SocketGuildUser;

        // ステータス（オンライン状態）
        switch (socketUser.Status)
        {
            case UserStatus.Online: status = ":green_circle:オンライン"; break;
            case UserStatus.Offline: status = ":black_circle:オフライン"; break;
            case UserStatus.Idle: status = ":crescent_moon:退席中"; break;
            case UserStatus.DoNotDisturb: status = ":no_entry:取り込み中"; break;
            default: status = "不明"; break;
        }

        // ActiveClients は IReadOnlySet<ClientType> 型なので、そのままループで使える
        if (socketUser.ActiveClients != null && socketUser.ActiveClients.Count > 0)
        {
            var platforms = socketUser.ActiveClients.Select(client =>
            {
                return client switch
                {
                    ClientType.Desktop => "🖥️デスクトップ",
                    ClientType.Mobile => "📱モバイル",
                    ClientType.Web => "🌐Web",
                    _ => client.ToString()
                };
            });

            platformInfo = string.Join(" / ", platforms);
        }
        else
        {
            platformInfo = "不明";
        }


        if (user.IsBot == true) // BOTか人間か区別する処理
        {
            isbot = ":robot:ボット";
        }
        else isbot = ":bust_in_silhouette:ユーザー";

        if (user.Nickname == null) // ユーザーのニックネーム処理
        {
            nickname = "未設定";
        }
        else nickname = user.Nickname; // ニックネームがある場合はそのまま表示

        // JSTに変換するためのタイムゾーン
        TimeZoneInfo jst = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");

        // 現在時刻（JST）
        DateTime nowJst = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, jst);

        // 差分（日数）
        int daysSinceCreated = (nowJst - user.CreatedAt.LocalDateTime).Days;
        int daysSinceJoined = (nowJst - user.JoinedAt.Value.LocalDateTime).Days;

        // ユーザーのロール取得（@everyoneロールを除外）
        var roles = user.RoleIds
            .Select(id => Context.Guild.GetRole(id))
            .Where(role => role != null && role.Id != Context.Guild.Id)
            .OrderByDescending(role => role.Position) // 上の方の役職順に並べる
            .ToList();

        string rolesText;

        if (roles.Count == 0)
        {
            rolesText = "なし";
        }
        else
        {
            rolesText = string.Join(", ", roles.Select(r => r.Mention));

            // EmbedのField制限（1024文字）対策
            if (rolesText.Length > 1000)
            {
                rolesText = string.Join(", ", roles.Select(r => r.Mention).TakeWhile((_, i) =>
                    string.Join(", ", roles.Select(r => r.Mention).Take(i)).Length < 1000)) + " ...他多数";
            }
        }

        var embedBuilder = new EmbedBuilder()
            .WithTitle($":mag: **{Context.User.GlobalName ?? Context.User.Username}さんの情報**")
            .AddField("ユーザー名", user.Username, true)
            .AddField("ユーザーID", user.Id, true)
            .AddField("アカウントの種類", isbot, true)
            .AddField("ステータス", $"{status}\n{platformInfo}", true)
            .AddField("アカウント登録日(JST)",  $"{user.CreatedAt.LocalDateTime}\n({daysSinceCreated}日前)", true)
            .AddField("サーバー参加日(JST)", $"{user.JoinedAt.Value.LocalDateTime}\n({daysSinceJoined}日前)", true)
            .AddField("ニックネーム", nickname, false)
            .AddField("所持ロール", rolesText, true)
            .WithThumbnailUrl(avatar)
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }
}