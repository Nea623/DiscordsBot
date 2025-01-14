using Discord.Commands;
using System.Net.NetworkInformation;

namespace DiscordBot.Modules.OtherModules;

public class PingModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "pingを計測します。")]
    public async Task PingCommandAsync(string url = "discord.com")
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
