namespace DiscordBot.Modules.ServerModules;

public class ServerIconModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // サーバーのアイコンを表示するコマンド
    // </summary>
    [SlashCommand("servericon", "サーバーのアイコンを表示します。")]
    public async Task ServerIconCommandAsync()
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("Icon Link")
            .WithUrl(Context.Guild.IconUrl)
            .WithImageUrl(Context.Guild.IconUrl)
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }
}
