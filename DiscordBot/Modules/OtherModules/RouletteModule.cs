namespace DiscordBot.Modules.OtherModules;
public class RouletteModule : InteractionModuleBase<SocketInteractionContext>
{
    private static readonly Random random = new();

    // <summary>
    // ルーレットコマンド
    // </summary>
    [SlashCommand("roulette", "選択肢からランダムに1つを選びます。(再試行ボタン付き)")]
    public async Task RouletteCommand([Summary(description: "カンマ区切りの選択肢(例: 寿司,ラーメン,ピザ)")] string options)
    {
        var items = options.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (items.Length < 2)
        {
            await RespondAsync("❌ 少なくとも2つ以上の選択肢を入力してください。(例: `/roulette 寿司,ラーメン,ピザ`)");
            return;
        }

        // 応答を遅延（表示中... を非表示にする）
        await DeferAsync();

        // 「回転中」風のメッセージを表示
        var reply = await FollowupAsync("🎰 回転中...", ephemeral: false);

        await Task.Delay(1500); // アニメーション風演出

        var result = items[random.Next(items.Length)];

        var embed = new EmbedBuilder()
            .WithTitle("🎲 ルーレット結果")
            .WithDescription($"**{result}** が選ばれました！")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E)
            .Build();

        // ボタン用カスタムID（選択肢とユーザーIDを含む）
        var customId = $"roulette_retry:{Context.User.Id}:{string.Join("|", items)}";

        var component = new ComponentBuilder()
            .WithButton("🔁 もう一回", customId, ButtonStyle.Primary)
            .Build();

        await reply.ModifyAsync(msg =>
        {
            msg.Content = string.Empty;
            msg.Embed = embed;
            msg.Components = component;
        });
    }

    // <summary>
    // 再試行ボタンを処理する関数（InteractionCreatedイベントから呼び出される想定）
    // </summary>
    public async Task HandleComponent(SocketMessageComponent component)
    {
        if (!component.Data.CustomId.StartsWith("roulette_retry:"))
            return;

        var parts = component.Data.CustomId.Split(':');
        if (parts.Length != 3)
            return;

        if (!ulong.TryParse(parts[1], out ulong originalUserId))
            return;

        if (component.User.Id != originalUserId)
        {
            // ❗ 応答前ならRespond、そうでなければFollowup
            if (!component.HasResponded)
                await component.RespondAsync("❌ このボタンはこのルーレットを実行したユーザー専用です。", ephemeral: true);
            else
                await component.FollowupAsync("❌ このボタンはこのルーレットを実行したユーザー専用です。", ephemeral: true);
            return;
        }

        var options = parts[2].Split('|');

        // ✅ 1. Defer（応答予約）→ これで3秒ルール回避
        await component.DeferAsync();

        // ✅ 2. 回転中アニメーション（メッセージを一時変更）
        await component.Message.ModifyAsync(msg =>
        {
            msg.Content = "🎰 回転中...";
            msg.Embed = null;
            msg.Components = new ComponentBuilder().Build(); // ボタン一時消去
        });

        await Task.Delay(2000); // アニメーション風待機

        var result = options[random.Next(options.Length)];

        var embed = new EmbedBuilder()
            .WithTitle("🎲 再ルーレット結果")
            .WithDescription($"**{result}** が選ばれました！")
            .WithFooter($"実行者: {component.User.GlobalName ?? component.User.Username}", component.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E)
            .Build();

        // ✅ 3. 最終結果に更新
        await component.Message.ModifyAsync(msg =>
        {
            msg.Content = "";
            msg.Embed = embed;
            msg.Components = new ComponentBuilder()
                .WithButton("🔁 もう一回", customId: component.Data.CustomId, style: ButtonStyle.Primary)
                .Build();
        });
    }
}