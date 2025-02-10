using Discord.Commands;

namespace DiscordBot.Modules.PermissionModules;

public class KickModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 指定したユーザーを追放するコマンド
    // </summary>
    [SlashCommand("kick", "指定したユーザーを追放します。")]
    [Discord.Commands.RequireBotPermission(GuildPermission.KickMembers)]
    [Discord.Commands.RequireUserPermission(GuildPermission.KickMembers)]
    public async Task KickCommandAsync([Discord.Interactions.Summary(description: "指定するユーザーを選択してください。")] SocketGuildUser targetUser, [Discord.Interactions.Summary(description: "理由を入力してください。")] [Remainder] string reason = null)
    {
        if (reason == null)
        {
            reason = "理由はありません";
        }
        await targetUser.KickAsync(reason);

        var embedBuilder = new EmbedBuilder()
            .WithTitle($"{targetUser.GlobalName ?? targetUser.Username}は追放されました:wave:")
            .WithDescription($"理由: {reason}")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }
}
