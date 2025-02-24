using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules.ServerModules;
public class InviteCreateModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // 無制限の招待リンクを作成して実行したチャンネルにピン留めするコマンド
    // </summary>
    [SlashCommand("invite", "無制限の招待リンクを作成して実行したチャンネルにピン留めします。")]
    public async Task InviteCreateCommandAsync()
    {
        var invite = await Context.Guild.SystemChannel.CreateInviteAsync(maxAge: null, maxUses: null);
        var message = await Context.Channel.SendMessageAsync($"無制限の招待リンクを作成しました。\n" + 
                                               $"{invite.Url}");
        await message.PinAsync();
    }
}
