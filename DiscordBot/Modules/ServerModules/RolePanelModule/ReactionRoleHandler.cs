using System.Text.RegularExpressions;

namespace DiscordBot.Modules.ServerModules.RolePanelModule;

public class ReactionRoleHandler
{
    public readonly DiscordSocketClient _client;

    public ReactionRoleHandler(DiscordSocketClient client)
    {
        _client = client;
    }

    public static async Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> cache, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        var message = await cache.GetOrDownloadAsync();
        if (message == null || !message.Embeds.Any()) return;

        var embed = message.Embeds.First();
        if (embed.Description == null) return;

        var lines = embed.Description.Split('\n');
        var matchedLine = lines.FirstOrDefault(line => line.Contains(reaction.Emote.Name));

        if (matchedLine == null) return;

        var match = Regex.Match(matchedLine, @"<@&(\d+)>");
        if (!match.Success) return;

        if (!ulong.TryParse(match.Groups[1].Value, out ulong roleId)) return;

        var guild = (reaction.Channel as SocketGuildChannel)?.Guild;
        if (guild == null) return;

        var role = guild.GetRole(roleId);
        if (role == null) return;

        var user = guild.GetUser(reaction.UserId);
        if (user == null || user.IsBot) return; // Botに対してロールを付与しない

        _ = Task.Run(async () =>
        {

            if (user.Roles.Any(x => x.Id == roleId))
            {
                await user.RemoveRoleAsync(role);

                var embedBuilder = new EmbedBuilder()
                    .WithDescription($"{user.Mention}から{role.Mention}を剥奪しました。")
                    .WithColor(0x8DCE3E);

                var msg = await message.ReplyAsync(embed: embedBuilder.Build());
                await Task.Delay(10000);
                await msg.DeleteAsync();
            }
            else
            {
                await user.AddRoleAsync(role);

                var embedBuilder = new EmbedBuilder()
                    .WithDescription($"{user.Mention}に{role.Mention}を付与しました。")
                    .WithColor(0x8DCE3E);

                var msg = await message.ReplyAsync(embed: embedBuilder.Build());
                await Task.Delay(10000);
                await msg.DeleteAsync();
            }
        });
        await message.RemoveReactionAsync(reaction.Emote, reaction.UserId);
    }
}
