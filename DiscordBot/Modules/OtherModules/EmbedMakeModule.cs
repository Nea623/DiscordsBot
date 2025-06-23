namespace DiscordBot.Modules.OtherModules;
public class EmbedMakeModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 高機能なEmbedを作成するコマンド
    // </summary>
    [SlashCommand("embedmake", "高機能なEmbedをBotが代わりに作ります。")]
    public async Task AdvancedEmbedMakeAsync([Summary(description: "タイトルを入力してください。")] string title,
                                             [Summary(description: "説明を入力してください。")] string description,

                                             [Summary(description: "カラーコード (例: #FF5733 または green など)")] string? color = null,
                                             [Summary(description: "サムネイル画像のURL")] string? thumbnailUrl = null,
                                             [Summary(description: "メイン画像のURL")] string? imageUrl = null,
                                             [Summary(description: "タイムスタンプを付けるか")] bool showTimestamp = false,

                                             [Summary(description: "フィールド1の名前")] string? field1Name = null,
                                             [Summary(description: "フィールド1の内容")] string? field1Value = null,
                                             [Summary(description: "フィールド2の名前")] string? field2Name = null,
                                             [Summary(description: "フィールド2の内容")] string? field2Value = null)
    {
        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(description)
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl());

        // カラー処理
        if (!string.IsNullOrWhiteSpace(color))
        {
            try
            {
                embed.WithColor(ParseColor(color));
            }
            catch
            {
                await RespondAsync("色の指定が無効です。例: `#FF0000` または `green`。");
                return;
            }
        }
        else
        {
            embed.WithColor(0x8DCE3E); // デフォルト
        }

        // サムネ・画像
        if (!string.IsNullOrWhiteSpace(thumbnailUrl))
            embed.WithThumbnailUrl(thumbnailUrl);

        if (!string.IsNullOrWhiteSpace(imageUrl))
            embed.WithImageUrl(imageUrl);

        if (showTimestamp)
            embed.WithCurrentTimestamp();

        // フィールド追加
        if (!string.IsNullOrWhiteSpace(field1Name) && !string.IsNullOrWhiteSpace(field1Value))
            embed.AddField(field1Name, field1Value);

        if (!string.IsNullOrWhiteSpace(field2Name) && !string.IsNullOrWhiteSpace(field2Value))
            embed.AddField(field2Name, field2Value);

        await RespondAsync(embed: embed.Build());
    }

    /// <summary>
    /// ユーザー入力から色を解析する
    /// </summary>
    private Color ParseColor(string input)
    {
        // HTMLカラーコード (#RRGGBB)
        if (input.StartsWith("#"))
        {
            return new Color(Convert.ToUInt32(input.Substring(1), 16));
        }

        // プリセット名
        return input.ToLower() switch
        {
            "red" => Color.Red,
            "green" => Color.Green,
            "blue" => Color.Blue,
            "orange" => Color.Orange,
            "purple" => Color.Purple,
            "gold" => Color.Gold,
            "teal" => Color.Teal,
            _ => throw new ArgumentException("不明な色名"),
        };
    }
}
