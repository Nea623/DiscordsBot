using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBot.Modules.DeveloperModules
{
    public class TestEmbedModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("testembed", "テスト用のEmbedを表示します（Bot開発者向け）。")]
        [RequireOwner] // Botオーナーのみ実行可能
        public async Task TestEmbedAsync()
        {
            var embed = new EmbedBuilder()
                .WithTitle("🧪 Embed テスト")
                .WithDescription("これはテスト用のEmbedです。\n色・装飾・整形を確認できます。")
                .AddField("フィールドA", "これはフィールドAの内容です。", true)
                .AddField("フィールドB", "これはフィールドBの内容です。", true)
                .AddField("マルチライン", "```csharp\nConsole.WriteLine(\"Hello World\");\n```", false)
                .WithThumbnailUrl("https://cdn-icons-png.flaticon.com/512/3500/3500833.png")
                .WithImageUrl("https://placehold.jp/600x200.png")
                .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
                .WithColor(Color.Purple)
                .WithCurrentTimestamp();

            await RespondAsync(embed: embed.Build());
        }
    }
}