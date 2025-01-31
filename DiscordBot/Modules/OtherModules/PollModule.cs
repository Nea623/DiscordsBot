namespace DiscordBot.Modules.OtherModules;

public class PollModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("poll", "アンケート表を作成します。")]
    public async Task PollCommandAsync([Summary(description: "タイトルを入力してください。")] string Title, string A = null, string B = null, string C = null, string D = null, string E = null, string F = null, string G = null, string H = null, string I = null, string J = null)
    {
        await DeferAsync();

        string[] iconUniCode = { "\uD83C\uDDE6",   // A
                                 "\uD83C\uDDE7",   // B
                                 "\uD83C\uDDE8",   // C
                                 "\uD83C\uDDE9",   // D
                                 "\uD83C\uDDEA",   // E
                                 "\uD83C\uDDEB",   // F
                                 "\uD83C\uDDEC",   // G
                                 "\uD83C\uDDED",   // H
                                 "\uD83C\uDDEE",   // I
                                 "\uD83C\uDDEF" }; // J

        var embed = new EmbedBuilder()
           .WithAuthor(Context.User.Username,
                       Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl());

        embed.WithTitle(Title);

        string description = null;

        string[] x = [A, B, C, D, E, F, G, H, I, J];
        x = x.Where(y => y != null).ToArray();

        IEmote[] emotes = new IEmote[x.Length];
        for (int n = 0; n < x.Length; n++)
        {
            description += (new Emoji(iconUniCode[n])).ToString() + " " + x[n] + "\n";
            emotes[n] = new Emoji(iconUniCode[n]);
        }
        embed.WithDescription(description);
        embed.WithCurrentTimestamp();
        embed.Color = new Color(0x8DCE3E);

        var message = await FollowupAsync(embed: embed.Build());
        await message.AddReactionsAsync(emotes);
    }
}