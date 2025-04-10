namespace DiscordBot.Modules.PermissionModules;

public class InviteCreateModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 無制限の招待リンクを作成して実行したチャンネルにピン留めするコマンド
    // </summary>
    [SlashCommand("invite", "無制限の招待リンクを作成して実行したチャンネルにピン留めします。")]
    [Discord.Commands.RequireBotPermission(GuildPermission.CreateInstantInvite)]
    [Discord.Commands.RequireUserPermission(GuildPermission.CreateInstantInvite)]
    public async Task InviteCreateCommandAsync()
    {
        var invite = await Context.Guild.SystemChannel.CreateInviteAsync(maxAge: null, maxUses: null);
        await RespondAsync("無制限の招待リンクを作成しました。\n" + 
                          $"{invite.Url}");
        var message = await GetOriginalResponseAsync();
        await message.PinAsync();
    }
}