﻿using Discord.Commands;

namespace DiscordBot.Modules.UserModules;

public class UserInfoModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("userinfo", "指定されたユーザーの情報を表示します。")]
    public async Task UserInfoCommandAsync([Remainder] IGuildUser user = null)
    {
        if (user is null) user = (IGuildUser)Context.User; // ユーザー未指定 --> 自分の情報が入る

        string avatar = user.GetAvatarUrl();
        string status = null;
        string isbot;
        string nickname;

        if (user.IsBot == true) // BOTか人間か区別する処理
        {
            isbot = "はい:robot:";
        }
        else isbot = "いいえ:bust_in_silhouette:";

        if (user.Nickname == null) // ユーザーのニックネーム処理
        {
            nickname = "なし";
        }
        else nickname = user.Nickname;

        if (user.Status == UserStatus.Online) status = "オンライン"; // ステータスの変換処理
        if (user.Status == UserStatus.Offline) status = "オフライン";
        if (user.Status == UserStatus.Idle) status = "退席中";
        if (user.Status == UserStatus.DoNotDisturb) status = "取り込み中";

        var embedBuilder = new EmbedBuilder()
            .WithTitle($"**{user}さんの情報**")
            .AddField("ユーザー名", user.Username)
            .AddField("ユーザーID", user.Id)
            .AddField("Botですか？", isbot)
            .AddField("ステータス", status)
            .AddField("アカウント登録日(JST)", user.CreatedAt.LocalDateTime)
            .AddField("サーバー参加日(JST)", user.JoinedAt.Value.LocalDateTime)
            .AddField("ニックネーム", nickname)
            .WithThumbnailUrl(avatar)
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }
}