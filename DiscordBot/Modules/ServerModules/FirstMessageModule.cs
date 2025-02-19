﻿namespace DiscordBot.Modules.ServerModules;

public class FirstMessageModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 実行したチャンネルの最初のメッセージを表示するコマンド
    // </summary>
    [SlashCommand("firstmessage", "実行したチャンネルの最初のメッセージを表示します。")]
    public async Task FirstMessageCommandAsync()
    {
        var message = await Context.Channel.GetMessagesAsync(0, Direction.After, 1).FlattenAsync();
        var embedBuilder = new EmbedBuilder()
            .WithTitle($"{Context.Channel.Name} での最初のメッセージ:speech_balloon:")
            .WithDescription(message.First().GetJumpUrl() +
                           $"\n送信された日時: {message.First().CreatedAt.LocalDateTime}")
            .WithColor(0x8DCE3E);
        await RespondAsync(embed: embedBuilder.Build());
    }
}
