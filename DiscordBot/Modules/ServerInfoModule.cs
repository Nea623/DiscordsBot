namespace DiscordBot.Modules;

public class ServerInfoModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("serverinfo", "サーバーの情報を表示します。")]
    public async Task ServerInfoCommandAsync()
    {
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
                             $"**ブーストレベル:** {Context.Guild.PremiumTier}\n" +
                             $"**サーバー作成日(JST):** {Context.Guild.CreatedAt.LocalDateTime}")
            .WithThumbnailUrl(Context.Guild.IconUrl)
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }
}