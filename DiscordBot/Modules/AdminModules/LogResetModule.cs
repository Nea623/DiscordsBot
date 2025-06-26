namespace DiscordBot.Modules.AdminModules;

public class LogResetModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // ログを消去するコマンド
    // </summary>
    [SlashCommand("reset", "ログを消去します。(開発者専用)")]
    [RequireOwner] // Botオーナーのみ実行可能
    public async Task LogResetCommandAsync()
    {
        Console.Clear();
        await RespondAsync("ログを消去しました:notepad_spiral:", ephemeral: true);
    }
}