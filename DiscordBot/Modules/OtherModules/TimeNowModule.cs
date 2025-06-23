namespace DiscordBot.Modules.OtherModules;

public class TimeNowModule : InteractionModuleBase<SocketInteractionContext>
{
    /// <summary>
    /// 現在の日時を表示するコマンド
    /// </summary>
    [SlashCommand("now", "現在の日時を表示します。")]
    public async Task NowCommandAsync()
    {
        var currentTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        await RespondAsync($"なう({currentTime})");
    }
}