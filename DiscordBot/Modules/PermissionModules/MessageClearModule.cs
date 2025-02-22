namespace DiscordBot.Modules.PermissionModules;

public class MessageClearModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 指定された数メッセージを削除するコマンド
    // </summary>
    [SlashCommand("clear", "指定された数メッセージを削除します。(権限必要)")]
    [RequireBotPermission(GuildPermission.ManageMessages)]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    public async Task MessageClearCommandAsync([Summary(description: "削除するメッセージの数を指定してください。")] int delNumber)
    {
        var channel = Context.Channel as SocketTextChannel;
        var items = await channel.GetMessagesAsync(delNumber).FlattenAsync();
        await channel.DeleteMessagesAsync(items);
        await RespondAsync($"{delNumber}個のメッセージを削除しました:wastebasket:");
    }
}
