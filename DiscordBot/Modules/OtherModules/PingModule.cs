using System.Net.NetworkInformation;

namespace DiscordBot.Modules.OtherModules;

public class PingModule : InteractionModuleBase<SocketInteractionContext>
{
    /* <summary>
    // 新pingコマンド(WebSocket PingとAPI Endpoint Pingを取得するもの)
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

        var latency = (DateTime.UtcNow - response.CreatedAt).Milliseconds;
        embedBuilder.Description += $"API Endpoint Ping: {latency}ms";

        await ModifyOriginalResponseAsync(x => x.Embed = embedBuilder.Build());
    }
    */

    // <summary>
    // 旧pingコマンド(ただ単に、アクセスできるかどうか、レスポンスタイムを取得しているだけのもの)
    // </summary>
    [SlashCommand("ping", "pingを計測行います。")]
    public async Task PingCommandAsync([Summary(description: "URLを指定してください。")] string url = "discord.com")
    {
        using (var ping = new Ping())
        {
            var result = ping.Send(url);

            if (result.Status == IPStatus.Success)
            {
                var embedBuilder = new EmbedBuilder()
                    .WithTitle("Pong! :ping_pong:")
                    .AddField("送信したURL", url, true)
                    .AddField("かかった時間", result.RoundtripTime + "ms", true)
                    .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
                    .WithColor(0x8DCE3E);

                await RespondAsync(embed: embedBuilder.Build());
            }
            else await RespondAsync("失敗: pingの送信に失敗しました。");
        }
    }
}