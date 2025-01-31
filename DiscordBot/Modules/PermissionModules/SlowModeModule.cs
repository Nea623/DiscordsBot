namespace DiscordBot.Modules.PermissionModules;

public class SlowModeModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("slowmode", "低速モードを指定した秒数で設定します。")]
    [RequireBotPermission(GuildPermission.ManageMessages)]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    public async Task SlowModeCommandAsync([Summary(description: "指定するチャンネルを選択してください。")] ITextChannel channel, [Summary(description: "秒数を指定してください。")] int perm)
    {
        await channel.ModifyAsync(x => x.SlowModeInterval = perm); // 低速モードをpermで指定された時間に設定

        if (perm == 0)
        {
            await RespondAsync($"チャンネル名: <#{channel.Id}> \n低速モードが解除されました:stopwatch:");
        }
        else await RespondAsync($"チャンネル名: <#{channel.Id}> \n低速モードが{perm}秒に設定されました:stopwatch:");
    }
}