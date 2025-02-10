﻿using Discord;

namespace DiscordBot.Services
{
    public class GuildService
    {
        // <summary>
        // 新規ユーザーが入室した際に歓迎メッセージを送信する
        // <summary>
        public async Task WelcomeMessageAsync(SocketGuildUser user)
        {
            SocketGuild guild = user.Guild; // ユーザーが入室したサーバーを取得
            string avatar = user.GetAvatarUrl(); // ユーザーのアバターURLを取得

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
