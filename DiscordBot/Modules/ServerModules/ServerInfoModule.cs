namespace DiscordBot.Modules.ServerModules;

public class ServerInfoModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // サーバーの情報を表示するコマンド
    // </summary>
    [SlashCommand("serverinfo", "サーバーの情報を表示します。")]
    public async Task ServerInfoCommandAsync()
    {
        string boost = null; // ブーストレベルを格納する変数

        if (Context.Guild.PremiumTier == PremiumTier.None) boost = "レベル0"; // ブーストレベルが0の場合
        if (Context.Guild.PremiumTier == PremiumTier.Tier1) boost = "レベル1"; // ブーストレベルが1の場合
        if (Context.Guild.PremiumTier == PremiumTier.Tier2) boost = "レベル2"; // ブーストレベルが2の場合
        if (Context.Guild.PremiumTier == PremiumTier.Tier3) boost = "レベル3"; // ブーストレベルが3の場合

        var embedBuilder = new EmbedBuilder()
            .WithTitle($"{Context.Guild.Name}の情報")
            .WithDescription($"**サーバー名:** {Context.Guild.Name}\n" +
                             $"**サーバーID:** {Context.Guild.Id}\n" +
                             $"**オーナー名:** {Context.Guild.Owner.Mention}\n" +
                             $"**オーナーID:** {Context.Guild.Owner.Id}\n" +
                             $"**メンバー数:** {Context.Guild.Users.Count(u => !u.IsBot)}\n" +
                             $"**Bot数:** {Context.Guild.Users.Count(u => u.IsBot)}\n" +
                             $"**チャンネル数:** {Context.Guild.Channels.Count}\n" +
                             $"**ロール数:** {Context.Guild.Roles.Count}\n" +
                             $"**ブーストレベル:** {boost} {Context.Guild.PremiumSubscriptionCount}ブースト\n" +
                             $"**サーバー作成日(JST):** {Context.Guild.CreatedAt.LocalDateTime}")
            .WithThumbnailUrl(Context.Guild.IconUrl)
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }
}