namespace DiscordBot.Modules.AdminModules;

public class LeaveServerModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 指定したサーバーからBotを脱退させるコマンド
    // </summary>
    [SlashCommand("leave", "指定したサーバーからBotを脱退させます。(開発者専用)")]
    public async Task LeaveServerCommandAsync([Summary(description: "サーバーIDを入力してください。")] string Id)
    {
        if (Context.User.Id == 1023888743360364606) // ユーザーIDを指定
        {
            var name = Context.Client.Guilds;
            IGuild guild = Context.Client.GetGuild(ulong.Parse(Id));
            if (guild == null)  // 指定したサーバーがない場合
            {
                await RespondAsync($"サーバー名: {guild.Name} は存在しません。", ephemeral: true);
                return;
            }
            await guild.LeaveAsync(); // 指定したサーバーがある場合は脱退する
            await RespondAsync($"サーバー名: {guild.Name} から脱退しました。", ephemeral: true);
        }
        else // 指定したユーザーIDでない場合
        {
            await RespondAsync("失敗: あなたは開発者コマンドを使用できません。", ephemeral: true);
        }
    }
}

