using Discord.Commands;
using System.Text;

namespace DiscordBot.Modules.ServerModules;

public class ServerInfoModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // サーバーの情報を表示するコマンド
    // </summary>
    [SlashCommand("serverinfo", "サーバーの情報を表示します。")]
    public async Task ServerInfoCommandAsync()
    {
        string boost = Context.Guild.PremiumTier switch
        {
            PremiumTier.Tier1 => "レベル1",
            PremiumTier.Tier2 => "レベル2",
            PremiumTier.Tier3 => "レベル3",
            _ => "レベル0"
        };

        var statusCounts = new Dictionary<UserStatus, int>();
        var guild = (Context.Client as DiscordSocketClient)?.GetGuild(Context.Guild.Id);

        foreach (var user in guild.Users)
        {
            var status = user.Status;
            if (statusCounts.ContainsKey(status))
                statusCounts[status]++;
            else
                statusCounts[status] = 1;
        }

        // ステータスの表示名（絵文字付き）取得
        string GetStatusDisplay(UserStatus status)
        {
            return status switch
            {
                UserStatus.Online => "🟢オンライン",
                UserStatus.Idle => "🌙退席中",
                UserStatus.DoNotDisturb => "⛔取り込み中",
                UserStatus.Offline or UserStatus.Invisible => "⚫オフライン",
                _ => "❓不明"
            };
        }

        // ステータス出力の優先順
        var displayOrder = new[]
        {
            UserStatus.Online,
            UserStatus.Idle,
            UserStatus.DoNotDisturb,
            UserStatus.Offline
        };

        // Invisible と Offline をまとめる
        int totalOffline = statusCounts.GetValueOrDefault(UserStatus.Offline) +
                           statusCounts.GetValueOrDefault(UserStatus.Invisible);

        var builder = new StringBuilder();
        foreach (var status in displayOrder)
        {
            int count = status switch
            {
                UserStatus.Offline => totalOffline,
                _ => statusCounts.GetValueOrDefault(status)
            };

            if (count > 0)
            {
                builder.AppendLine($"- {GetStatusDisplay(status)}: {count}人");
            }
        }

        var createdAt = Context.Guild.CreatedAt.LocalDateTime;
        var daysAgo = (DateTime.Now - createdAt).Days;

        var embedBuilder = new EmbedBuilder()
            .WithTitle($"{Context.Guild.Name}の情報")
            .AddField("サーバーID", Context.Guild.Id)
            .AddField("所有者", Context.Guild.Owner.Mention)
            .AddField("サーバー作成日 (JST)", $"{createdAt:yyyy/MM/dd HH:mm:ss}（{daysAgo}日前）")
            .AddField("人数", $"{Context.Guild.Users.Count}人 (ユーザー: {Context.Guild.Users.Count(u => !u.IsBot)}人 / Bot: {Context.Guild.Users.Count(u => u.IsBot)}人)")
            .AddField("ステータス別人数", builder.ToString())
            .AddField("その他", $"チャンネル数: {Context.Guild.Channels.Count}個\n" + 
                                $"ロール数: {Context.Guild.Roles.Count}個\n" + 
                                $"ブーストレベル: {boost}({Context.Guild.PremiumSubscriptionCount}ブースト)")
            
            .WithThumbnailUrl(Context.Guild.IconUrl)
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }
}