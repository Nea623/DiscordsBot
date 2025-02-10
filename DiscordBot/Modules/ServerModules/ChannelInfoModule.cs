namespace DiscordBot.Modules.ServerModules;

public class ChannelInfoModule : InteractionModuleBase<SocketInteractionContext>
{
    // <summary>
    // チャンネルの情報を表示するコマンド
    // </summary>
    [SlashCommand("channelinfo", "チャンネルの情報を表示します。")]
    public async Task ChannelInfoCommandAsync([Summary(description: "指定するチャンネルまたはカテゴリを選択してください。")] IChannel channel)
    {
        var embedBuilder = new EmbedBuilder()
            .WithDescription($"<#{channel.Id}>")
            .AddField("チャンネル名", $"**{channel.Name}**({channel.Id})")
            .WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl())
            .WithColor(0x8DCE3E);

        if (channel.GetChannelType() == ChannelType.Text) // テキストチャンネルの場合
            await AddTextChannelFieldsAsync(embedBuilder, channel as ITextChannel); 
        else if (channel.GetChannelType() == ChannelType.Voice) // ボイスチャンネルの場合
            AddVoiceChannelFields(embedBuilder, channel as IVoiceChannel);

        await RespondAsync(embed: embedBuilder.Build());
    }

    // <summary>
    // テキストチャンネルの情報を埋め込みに追加する
    // </summary>
    private async Task AddTextChannelFieldsAsync(EmbedBuilder embedBuilder, ITextChannel textChannel)
    {
        var category = await textChannel.GetCategoryAsync();
        embedBuilder
            .AddField("カテゴリ", $"{category.Name} ({category.Id})")
            .AddField("トピック", textChannel.Topic ?? "なし")
            .AddField("NSFW", textChannel.IsNsfw ? "はい" : "いいえ")
            .AddField("クールダウン", textChannel.SlowModeInterval == 0 ? "なし" : $"{textChannel.SlowModeInterval}秒");
    }

    // <summary>
    // ボイスチャンネルの情報を埋め込みに追加する
    // </summary>
    private void AddVoiceChannelFields(EmbedBuilder embedBuilder, IVoiceChannel voiceChannel)
    {
        embedBuilder
            .AddField("ビットレート", $"{voiceChannel.Bitrate / 1000}kbps")
            .AddField("定員", voiceChannel.UserLimit.HasValue ? voiceChannel.UserLimit.Value.ToString() : "指定なし");
    }
}