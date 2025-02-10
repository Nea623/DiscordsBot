using Discord;

namespace DiscordBot.Modules.ServerModules;

public class EmojiGetModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 絵文字を画像として表示するコマンド
    // </summary>
    [SlashCommand("emojiget", "指定した絵文字を画像として表示します。")]
    public async Task EmojiGetCommandAsync([Summary(description: "表示したい絵文字を『テキストをコピー』でコピペしてください。")] string emote_value)
    {
        var emote = Emote.Parse(emote_value); // 絵文字を解析して取得します。
        var embedBuilder = new EmbedBuilder()
            .WithTitle("絵文字情報")
            .WithDescription($"絵文字名: {emote.Name}\n絵文字ID: {emote.Id}")
            .WithImageUrl(emote.Url);

        await RespondAsync(embed: embedBuilder.Build());
    }
}