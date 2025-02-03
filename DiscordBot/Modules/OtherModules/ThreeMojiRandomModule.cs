namespace DiscordBot.Modules.ServerModules;

public class ThreeMojiRandomModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("3moji", "ひらがな3文字をランダムに表示します。")]
    public async Task ThreeMojiCommandAsync()
    {
        var random = new Random();
        var moji = new string[] { "あ", "い", "う", "え", "お",
                                  "か", "き", "く", "け", "こ",
                                  "さ", "し", "す", "せ", "そ",
                                  "た", "ち", "つ", "て", "と",
                                  "な", "に", "ぬ", "ね", "の",
                                  "は", "ひ", "ふ", "へ", "ほ",
                                  "ま", "み", "む", "め", "も",
                                  "や", "ゆ", "よ",
                                  "ら", "り", "る", "れ", "ろ",
                                  "わ", "を", "ん",
                                  "が", "ぎ", "ぐ", "げ", "ご",
                                  "ざ", "じ", "ず", "ぜ", "ぞ",
                                  "だ", "ぢ", "づ", "で", "ど",
                                  "ば", "び", "ぶ", "べ", "ぼ",
                                  "ぱ", "ぴ", "ぷ", "ぺ", "ぽ",
                                  "ぁ", "ぃ", "ぅ", "ぇ", "ぉ",
                                  "ゃ", "ゅ", "ょ", "っ" };



        var result = string.Join("", moji.OrderBy(_ => random.Next()).Take(3));
        await RespondAsync(result);
    }
}