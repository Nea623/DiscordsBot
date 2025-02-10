namespace DiscordBot.Modules.OtherModules;

[Group("cal", "cal commands - group.")]
public class CalculationModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 加法を行うコマンド
    // </summary>
    [SlashCommand("add", "足し算をします。")]
    public async Task AddCommandAsync(int b, int c)
    {
        int a;

        a = b + c;

        var embedBuilder = new EmbedBuilder()
            .WithTitle("計算結果(加法)")
            .WithDescription($"{b} + {c} = **{a}**")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }

    // <summary>
    // 減法を行うコマンド
    // </summary>
    [SlashCommand("sub", "引き算をします。")]
    public async Task SubCommandAsync(int b, int c)
    {
        int a;

        a = b - c;

        var embedBuilder = new EmbedBuilder()
            .WithTitle("計算結果(減法)")
            .WithDescription($"{b} - {c} = **{a}**")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }

    // <summary>
    // 乗法を行うコマンド
    // </summary>
    [SlashCommand("mul", "掛け算をします。")]
    public async Task MulCommandAsync(int b, int c)
    {
        int a;

        a = b * c;

        var embedBuilder = new EmbedBuilder()
            .WithTitle("計算結果(乗法)")
            .WithDescription($"{b} × {c} = **{a}**")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }

    // <summary>
    // 除法を行うコマンド
    // </summary>
    [SlashCommand("div", "割り算をします。")]
    public async Task DivCommandAsync(int b, int c)
    {
        int a;

        if (c == 0)
        {
            await RespondAsync("失敗: 0では割れません。");
            return;
        }
        else
        {
            a = b / c;

            var embedBuilder = new EmbedBuilder()
                .WithTitle("計算結果(除法)")
                .WithDescription($"{b} ÷ {c} = **{a}**")
                .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
                .WithColor(0x8DCE3E);

            await RespondAsync(embed: embedBuilder.Build());
        }
    }
}