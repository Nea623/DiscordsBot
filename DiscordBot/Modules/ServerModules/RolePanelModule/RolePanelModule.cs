using System.Text.RegularExpressions;

namespace DiscordBot.Modules.ServerModules.RolePanelModule;

[Group("rolepanel", "rolepanel commands - group.")]
public class RolePanelModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 役職パネルを作成するコマンド
    // </summary>
    [SlashCommand("create", "役職パネルを作成します。")]
    public async Task RolePanelCommandAsync([Summary(description: "タイトルを入力してください。")] string title, [Summary(description: "追加するロールを選択してください。")] IRole role)
    {
        var embedbuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription($"🇦 {role.Mention}")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedbuilder.Build());
        var message = await Context.Interaction.GetOriginalResponseAsync();
        await message.AddReactionAsync(new Emoji("🇦"));
    }

    // <summary>
    // 役職パネルを削除するコマンド
    // </summary>
    [SlashCommand("delete", "役職パネルを削除します。")]
    public async Task DeleteRolePanelCommandAsync()
    {
        var messages = await Context.Channel.GetMessagesAsync(10).FlattenAsync();
        var lastEmbedMessage = messages.FirstOrDefault(msg => msg.Embeds.Any());
        if (lastEmbedMessage == null)
        {
            await RespondAsync("役職パネルが見つかりませんでした。", ephemeral: true);
            return;
        }
        if (lastEmbedMessage is IUserMessage userMessage)
        {
            await userMessage.DeleteAsync();
            await RespondAsync("役職パネルを削除しました。", ephemeral: true);
        }
        else
        {
            await RespondAsync("処理に失敗しました。", ephemeral: true); // 何かしらの処理が失敗した場合
        }
    }

    // <summary>
    // 役職パネルに役職を追加するコマンド
    // </summary>
    [SlashCommand("add", "指定した役職を役職パネルに追加します。")]
    public async Task AddRoleToLastEmbedAsync([Summary(description: "役職パネルに追加するロールを指定してください。")] IRole role)
    {
        var messages = await Context.Channel.GetMessagesAsync(10).FlattenAsync();
        var lastEmbedMessage = messages.FirstOrDefault(msg => msg.Embeds.Any());

        if (lastEmbedMessage == null)
        {
            await RespondAsync("役職パネルが見つかりませんでした。", ephemeral: true);
            return;
        }

        string[] emoji = { "\uD83C\uDDE6",   // A
                           "\uD83C\uDDE7",   // B
                           "\uD83C\uDDE8",   // C
                           "\uD83C\uDDE9",   // D
                           "\uD83C\uDDEA",   // E
                           "\uD83C\uDDEB",   // F
                           "\uD83C\uDDEC",   // G
                           "\uD83C\uDDED",   // H
                           "\uD83C\uDDEE",   // I
                           "\uD83C\uDDEF" }; // J
        var nextEmoji = GetNextAvailableEmoji(emoji, lastEmbedMessage.Embeds.First().Description);

        if (nextEmoji == null)
        {
            await RespondAsync("これ以上役職を追加できません。", ephemeral: true);
            return;
        }

        var nextReaction = new Emoji(nextEmoji);
        var embed = lastEmbedMessage.Embeds.First().ToEmbedBuilder();
        embed.Description += $"\n{nextEmoji} {role.Mention}";

        if (lastEmbedMessage is IUserMessage userMessage)
        {
            await userMessage.ModifyAsync(msg => msg.Embed = embed.Build()); // 埋め込みメッセージを更新
            await userMessage.AddReactionAsync(nextReaction); // リアクションを追加
            await DeferAsync(true); // 応答を3秒遅延させる
            await DeleteOriginalResponseAsync(); // 既存の応答を削除
        }
        else
        {
            await RespondAsync("処理に失敗しました。", ephemeral: true); // 何かしらの処理が失敗した場合
        }
    }

    // <summary>
    // 役職パネルから役職を削除するコマンド
    // </summary>
    [SlashCommand("remove", "指定した役職を役職パネルから削除します。")]
    public async Task RemoveRolePanelList([Summary(description: "削除するロールを指定してください。")] IRole role)
    {
        var messages = await Context.Channel.GetMessagesAsync(10).FlattenAsync();
        var lastEmbedMessage = messages.FirstOrDefault(msg => msg.Embeds.Any());
        if (lastEmbedMessage == null)
        {
            await RespondAsync("役職パネルが見つかりませんでした。", ephemeral: true);
            return;
        }

        var embed = lastEmbedMessage.Embeds.First().ToEmbedBuilder();
        var lines = embed.Description.Split('\n').ToList();
        var matchedLine = lines.FirstOrDefault(line => line.Contains(role.Id.ToString()));

        if (matchedLine == null)
        {
            await RespondAsync("指定された役職が見つかりませんでした。", ephemeral: true);
            return;
        }

        var roleId = GetRoleId(matchedLine);
        if (roleId == null)
        {
            await RespondAsync("役職IDの取得に失敗しました。", ephemeral: true);
            return;
        }

        lines.Remove(matchedLine);
        embed.Description = string.Join('\n', lines);

        if (lastEmbedMessage is IUserMessage userMessage)
        {
            await userMessage.ModifyAsync(msg => msg.Embed = embed.Build()); // 埋め込みメッセージを更新
            var emoji = matchedLine.Split(':')[0].Trim();
            var reaction = new Emoji(emoji);
            await userMessage.RemoveReactionAsync(reaction, Context.Client.CurrentUser); // リアクションを削除
            await RespondAsync("役職パネルから役職を削除しました。", ephemeral: true);
        }
        else
        {
            await RespondAsync("処理に失敗しました。", ephemeral: true); // 何かしらの処理が失敗した場合
        }
    }

    // <summary>
    // 次に使用可能な絵文字を取得します。
    // </summary>
    public string GetNextAvailableEmoji(string[] emoji, string description)
    {
        foreach (var em in emoji)
        {
            if (!description.Contains(em))
            {
                return em;
            }
        }
        return null; // すべての絵文字が使用されている場合
    }

    public ulong? GetRoleId(string str)
    {
        var match = Regex.Match(str, @"<@&(\d+)>");
        if (!match.Success) return null;
        if (!ulong.TryParse(match.Groups[1].Value, out ulong roleId)) return null;

        return roleId;
    }

}
