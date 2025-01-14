namespace DiscordBot.Modules.PermissionModules;

public class MessageClearModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("clear", "指定された数メッセージを削除します。")]
    [RequireBotPermission(GuildPermission.ManageMessages)]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    public async Task MessageClearCommandAsync(int delNumber)
    {
        var channel = Context.Channel as SocketTextChannel;
        var items = await channel.GetMessagesAsync(delNumber).FlattenAsync();
        await channel.DeleteMessagesAsync(items);
        await RespondAsync($"{delNumber}個のメッセージを削除しました:wastebasket:");
    }
}
