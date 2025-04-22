using System.Net.NetworkInformation;

namespace DiscordBot.Modules.OtherModules;

public class PingModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // pingコマンド(WebSocket PingとAPI Endpoint Pingを取得するもの)
    // </summary>
    [IntegrationType(ApplicationIntegrationType.GuildInstall | ApplicationIntegrationType.UserInstall)]
    [SlashCommand("ping", "pingの計測を行います。")]
    public async Task PingCommandAsync()
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("Pong! :ping_pong:")
            .WithDescription($"WebSocket Ping: {Context.Client.Latency}ms\n")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
        var response = await GetOriginalResponseAsync();

        var latency = ((DateTime.Now + TimeSpan.FromSeconds(1)) - response.CreatedAt).Milliseconds;
        embedBuilder.Description += $"API Endpoint Ping: {latency}ms";

        await ModifyOriginalResponseAsync(x => x.Embed = embedBuilder.Build());
    }
}