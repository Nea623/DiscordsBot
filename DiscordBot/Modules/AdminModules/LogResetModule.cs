namespace DiscordBot.Modules.AdminModules;

public class LogResetModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // ログを消去するコマンド
    // </summary>
    [SlashCommand("reset", "ログを消去します。(開発者専用)")]
    public async Task LogResetCommandAsync()
    {
        if (Context.User.Id == 1023888743360364606)
        {
            Console.Clear();
            await RespondAsync("ログを消去しました:notepad_spiral:", ephemeral: true);
        }
        else
        {
            await RespondAsync("失敗: あなたは開発者コマンドを使用できません。", ephemeral: true);
        }
    }
}