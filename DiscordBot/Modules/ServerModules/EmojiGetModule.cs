using Discord;

namespace DiscordBot.Modules.ServerModules;

public class EmojiGetModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("emojiget", "指定した絵文字を画像として表示します。")]
    public async Task EmojiGetCommandAsync([Summary(description: "表示したい絵文字を『テキストをコピー』でコピペしてください。")] string emote_value)
    {
        var emote = Emote.Parse(emote_value);
        var embedBuilder = new EmbedBuilder()
            .WithTitle("絵文字情報")
            .WithDescription($"絵文字名: {emote.Name}\n絵文字ID: {emote.Id}")
            .WithImageUrl(emote.Url);

        await RespondAsync(embed: embedBuilder.Build());
    }
}