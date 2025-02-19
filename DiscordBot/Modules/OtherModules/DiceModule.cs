namespace DiscordBot.Modules.OtherModules;

public class DiceModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // サイコロを振るコマンド
    // </summary>
    [SlashCommand("dice", "サイコロを振ります。")]
    public async Task DiceCommandAsync([Summary(description: "振るサイコロの面数を入力してください。")] int DiceNumber = 0)
    {
        if (DiceNumber <= 0) DiceNumber = 6;
        var random = new Random();

        var result = random.Next(1, DiceNumber + 1);
        await RespondAsync(result.ToString());
    }
}
