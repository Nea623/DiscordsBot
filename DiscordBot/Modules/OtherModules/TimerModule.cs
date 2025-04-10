using Discord.Commands;

namespace DiscordBot.Modules.OtherModules;

public class TimerModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // タイマーをセットするコマンド
    // </summary>
    [SlashCommand("timer", "タイマーをセットします。")]
    public async Task TimerCommandAsync([Discord.Interactions.Summary(description: "m=分, h=時を数字の後に入力してください。")][Remainder] TimeSpan TimerTime)
    {
        if (TimerTime.TotalSeconds <= 0) TimerTime = TimeSpan.FromSeconds(1);
        await RespondAsync($"タイマーを{TimerTime.TotalSeconds}秒にセットしました。\n" +
                           $"{DateTime.Now.AddSeconds(TimerTime.TotalSeconds)}に予定されています。");
        await Task.Delay(TimerTime);
        await FollowupAsync($"{Context.User.Mention} タイマーが終了しました。");
    }
}