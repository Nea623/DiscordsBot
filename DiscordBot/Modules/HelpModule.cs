namespace DiscordBot.Modules;

public class HelpModule(InteractionService interactionService) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly InteractionService _interactionService = interactionService;

    [AutocompleteCommand("command_name", "help")]
    public async Task Autocomplete()
    {
        var results = new List<AutocompleteResult>();

        string? userInput = ((SocketAutocompleteInteraction)Context.Interaction).Data.Current.Value.ToString();

        foreach (var slashCommand in _interactionService.SlashCommands)
        {
            var groupName = !string.IsNullOrEmpty(slashCommand.Module.SlashGroupName) ? $"{slashCommand.Module.SlashGroupName} " : "";
            var groupName2 = !string.IsNullOrEmpty(slashCommand.Module.Parent?.SlashGroupName) ? $"{slashCommand.Module.Parent?.SlashGroupName} " : "";
            var hashCode = slashCommand.GetHashCode();

            results.Add(new AutocompleteResult($"{groupName2}{groupName}{slashCommand.Name}", $"{hashCode}"));
        }

        // ユーザーの入力と一致するものをフィルタリング
        results = results.Where(x => x.Name.StartsWith(userInput, StringComparison.OrdinalIgnoreCase)).ToList();

        // 一度に最大 25 件の提案
        await ((SocketAutocompleteInteraction)Context.Interaction).RespondAsync(results.Take(25));
    }

    // <summary>
    // ヘルプを表示するコマンド
    // <summary>
    [SlashCommand("help", "ヘルプを表示します。")]
    public async Task HelpModuleCommandAsync([Summary("command_name"), Autocomplete] string? command_name = null)
    {
        var embed = new EmbedBuilder();

        if (command_name == null)
        {
            embed.WithTitle(":information_source: | HANaBi -はなび- のヘルプ");
            embed.WithDescription("特定のコマンドの詳細情報は `/help [コマンド名]` を入力してください。");
            embed.AddField("サーバー数/Ping", $"`{Context.Client.Guilds.Count}`/`{Context.Client.Latency}ms`");
            embed.AddField("Botを招待/サポートサーバー", "[`Botを招待`](https://discord.com/oauth2/authorize?client_id=1325369554456674406&permissions=8&integration_type=0&scope=bot)/[`サポートサーバーに入る`](https://discord.gg/hSfV6CDFXy)");
            embed.WithFooter($"実行者: {Context.User.GlobalName ?? Context.User.Username}", Context.User.GetDisplayAvatarUrl());
            embed.WithColor(0x8DCE3E);
        }
        else
        {
            var commandInfo = _interactionService.SlashCommands.FirstOrDefault(x => x.GetHashCode().ToString() == command_name); // コマンド名からコマンド情報を取得
            if (commandInfo == null)
            {
                await RespondAsync("コマンドが見つかりませんでした。");
                return;
            }

            var groupName = !string.IsNullOrEmpty(commandInfo.Module.SlashGroupName) ? $"{commandInfo.Module.SlashGroupName} " : "";
            var groupName2 = !string.IsNullOrEmpty(commandInfo.Module.Parent?.SlashGroupName) ? $"{commandInfo.Module.Parent?.SlashGroupName} " : "";




            embed.WithTitle($"コマンド詳細 - {groupName2}{groupName}{commandInfo.Name}")
                .WithDescription($"{commandInfo.Description}")
                .AddField("使用例", $"`{groupName2}{groupName}{commandInfo.Name}`");
        }

        await RespondAsync(embed: embed.Build());
    }
}