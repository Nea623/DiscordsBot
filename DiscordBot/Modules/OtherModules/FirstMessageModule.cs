namespace DiscordBot.Modules.OtherModules;

public class FirstMessageModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("firstmessage", "実行したチャンネルの最初のメッセージを表示します。")]
    public async Task FirstMessageCommandAsync()
    {
        var message = await Context.Channel.GetMessagesAsync(0, Direction.After, 1).FlattenAsync();
        await RespondAsync(message.First().GetJumpUrl());
    }
}
