namespace DiscordBot.Modules.UserModules;

public class UserAvatarModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("avatar", "指定されたユーザーのアバターを表示します。")]
    public async Task UserAvatarCommandAsync([Summary(description: "指定するユーザーを選択してください。")] IUser? user = null)
    {
        user = user ?? Context.User;

        var embedBuilder = new EmbedBuilder()
            .WithTitle("Avatar Link")
            .WithUrl(user.GetAvatarUrl(size: 1024))
            .WithImageUrl(user.GetAvatarUrl(size: 1024))
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        await RespondAsync(embed: embedBuilder.Build());
    }
}