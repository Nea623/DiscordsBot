namespace DiscordBot.Modules.AdminModules;
public class JoinedServerModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // Botが導入されているサーバーを一覧として表示するコマンド
    // </summary>
    [SlashCommand("joined", "Botが導入されているサーバーを一覧として表示します。(開発者専用)")]
    public async Task JoinedServerCommandAsync()
    {
        var guilds = Context.Client.Guilds;
        EmbedBuilder embedBuilder = new EmbedBuilder();

        if (Context.User.Id == 1287268215499784242) // ユーザーIDを指定
        {
            embedBuilder.Title = "導入されているサーバー";

            for (int i = 0; i < guilds.Count(); i++)
            {
                embedBuilder.Description += "サーバー名: " + "[" + guilds.Skip(i).FirstOrDefault().Name + "]" + " / " + "サーバーID: " + "[" + guilds.Skip(i).FirstOrDefault().Id + "]" + "\n";
            }
            embedBuilder.WithColor(0x8DCE3E);
            await RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
        }
        else // 指定したユーザーIDでない場合
        {
            await RespondAsync("失敗: あなたは開発者コマンドを使用できません。", ephemeral: true);
        }
    }
}