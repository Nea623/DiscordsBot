using Discord;

namespace DiscordBot.Modules.OtherModules;   

public class SearchModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // Google検索を行うコマンド
    // </summary>
    [SlashCommand("search", "Google検索を行います。")]
    public async Task SearchCommandAsync([Summary(description: "検索したいキーワードを入力してください。")] string keyword)
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle($"{keyword}の検索結果です。")
            .WithDescription($"🔍[リンクはこちら](https://www.google.com/search?q={keyword})")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);
        await RespondAsync(embed: embedBuilder.Build());
    }
}