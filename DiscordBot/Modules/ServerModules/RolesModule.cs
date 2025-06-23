using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Text;

public class RoleModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("roles", "このサーバーのロール一覧を表示します。")]
    public async Task RolesCommandAsync()
    {
        var guild = Context.Guild;
        if (guild == null)
        {
            await RespondAsync("サーバーが見つかりません。", ephemeral: true);
            return;
        }

        var roles = guild.Roles
            .Where(r => !r.IsEveryone)
            .OrderByDescending(r => r.Position)
            .ToList();

        if (roles.Count == 0)
        {
            await RespondAsync("ロールが存在しません。", ephemeral: true);
            return;
        }

        int totalMembers = guild.MemberCount;
        var embed = BuildRolesEmbed(roles, totalMembers, page: 0, pageSize: 20);
        var components = BuildPaginationButtons(page: 0, totalPages: (int)Math.Ceiling(roles.Count / 20.0));

        await RespondAsync(embed: embed, components: components);
    }

    // Static: ボタンイベントで呼び出し可能
    public static async Task HandleRolePaginationAsync(SocketMessageComponent component, DiscordSocketClient client)
    {
        var guild = client.GetGuild(component.GuildId ?? 0);
        if (guild == null)
        {
            await component.RespondAsync("サーバー情報が取得できませんでした。", ephemeral: true);
            return;
        }

        var roles = guild.Roles
            .Where(r => !r.IsEveryone)
            .OrderByDescending(r => r.Position)
            .ToList();

        int totalMembers = guild.MemberCount;
        int pageSize = 20;
        int totalPages = (int)Math.Ceiling(roles.Count / (double)pageSize);

        // CustomId: "roles_next_0" や "roles_prev_1" 形式
        var parts = component.Data.CustomId.Split('_');
        string action = parts[1];
        int currentPage = int.Parse(parts[2]);

        int newPage = action == "next" ? currentPage + 1 : currentPage - 1;
        newPage = Math.Clamp(newPage, 0, totalPages - 1);

        var embed = BuildRolesEmbed(roles, totalMembers, newPage, pageSize);
        var components = BuildPaginationButtons(newPage, totalPages);

        await component.UpdateAsync(msg =>
        {
            msg.Embed = embed;
            msg.Components = components;
        });
    }

    // Embed を構築
    private static Embed BuildRolesEmbed(List<SocketRole> roles, int totalMembers, int page, int pageSize)
    {
        var builder = new StringBuilder();
        var pagedRoles = roles.Skip(page * pageSize).Take(pageSize);

        foreach (var role in pagedRoles)
        {
            int count = role.Members.Count();
            double percentage = totalMembers == 0 ? 0 : (double)count / totalMembers * 100;
            builder.AppendLine($"- {role.Mention}: {count}人 / {percentage:F1}%");
        }

        return new EmbedBuilder()
            .WithTitle($"📜 ロール一覧（{page + 1}ページ）")
            .WithDescription(builder.ToString())
            .WithColor(Color.Green)
            .WithFooter($"総メンバー数: {totalMembers}人")
            .Build();
    }

    // ボタン生成
    private static MessageComponent BuildPaginationButtons(int page, int totalPages)
    {
        var builder = new ComponentBuilder();

        builder.WithButton("◀ 前へ", $"roles_prev_{page}", ButtonStyle.Primary, disabled: page <= 0);
        builder.WithButton("次へ ▶", $"roles_next_{page}", ButtonStyle.Primary, disabled: page >= totalPages - 1);

        return builder.Build();
    }
}
