namespace DiscordBot.Modules.OtherModules;

public class FirstMessageModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 実行したチャンネルの最初のメッセージを表示するコマンド
    // </summary>
    [SlashCommand("firstmessage", "実行したチャンネルの最初のメッセージを表示します。")]
    public async Task FirstMessageCommandAsync()
    {
        var message = await Context.Channel.GetMessagesAsync(0, Direction.After, 1).FlattenAsync();
        await RespondAsync(message.First().GetJumpUrl());
    }
}
