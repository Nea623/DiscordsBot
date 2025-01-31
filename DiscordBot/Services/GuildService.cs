using Discord;

namespace DiscordBot.Services
{
    public class GuildService
    {
        public async Task WelcomeMessageAsync(SocketGuildUser user)
        {
            SocketGuild guild = user.Guild;
            string avatar = user.GetAvatarUrl();

            var embedBuilder = new EmbedBuilder()
                .WithTitle("新規ユーザーが入室しました！")
                .WithDescription($"{user.Mention}さん、**{user.Guild.Name}**へようこそ！\n" +
                                 $"あなたは{user.Guild.MemberCount - guild.Users.Count(x => x.IsBot)}人目のメンバーです。\n" +
                                 $"新規さんを歓迎しよう🎉")
                .WithThumbnailUrl(avatar)
                .WithColor(0x8DCE3E);

            await (user.Guild.SystemChannel).SendMessageAsync(embed: embedBuilder.Build());
        }
    }
}
