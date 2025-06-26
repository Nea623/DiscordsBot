using Discord;

namespace DiscordBot.Modules.UserModules;

public class UserBannerModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 指定されたユーザーのバナーを表示するコマンド
    // </summary>
    [SlashCommand("banner", "指定ユーザーのバナー画像を表示します。")]
    public async Task BannerCommandAsync(SocketUser? user = null)
    {
        user ??= Context.User;

        // RestUserに変換してバナーを取得
        var restUser = await Context.Client.Rest.GetUserAsync(user.Id);

        if (restUser.BannerId != null)
        {
            var bannerUrl = restUser.GetBannerUrl(ImageFormat.Auto, 1024);
            var embedBuilder = new EmbedBuilder()
                .WithTitle("Banner Link")
                .WithUrl(bannerUrl)
                .WithImageUrl(bannerUrl)
                .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
                .WithColor(0x8DCE3E);

            await RespondAsync(embed: embedBuilder.Build());
        }
        else
        {
            await RespondAsync($"{user.Username} さんはバナーを設定していません。");
        }
    }
}