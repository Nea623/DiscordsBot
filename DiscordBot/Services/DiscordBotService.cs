using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using DiscordBot.Modules.ServerModules.RolePanelModule;
using Microsoft.Win32.SafeHandles;

namespace DiscordBot.Services;

public class DiscordBotService(DiscordSocketClient client, InteractionService interactions, InteractionHandler interactionHandler,
    IConfiguration configuration, ILogger<DiscordBotService> logger) : BackgroundService
{
    // ユーザーIDごとにその日のリアクション済み日を記録
    private Dictionary<ulong, DateTime> _userReactionDates = new Dictionary<ulong, DateTime>();
    // '_dailyResetTimer' を null 許容型に変更することで、CS8618 エラーを解消します。  
    private Timer _dailyResetTimer;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        client.Log += Log;
        interactions.Log += Log;

        client.Ready += Ready;
        client.UserJoined += EventHandler;
        client.ReactionAdded += ReactionRoleHandler.OnReactionAddedAsync;
        client.MessageReceived += MessageReceivedAsync;
        client.MessageReceived += SendGreetingMessage;




        await interactionHandler.InitializeAsync();
        await client.LoginAsync(TokenType.Bot, configuration["DiscordBot:Token"]);
        await client.StartAsync();
        //Botのステータス
        await client.SetStatusAsync(UserStatus.AFK);
        //カスタムステータス
        await client.SetGameAsync("/help");

        // タイマーの初期化
        InitializeDailyResetTimer();


        await Task.Delay(-1, cancellationToken);
    }

    // <summary>
    // Botの停止
    // </summary>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        if (ExecuteTask == null)
            return Task.CompletedTask;

        base.StopAsync(cancellationToken);
        return client.StopAsync();
    }


    // <summary>
    // ログ出力
    // </summary>
    public Task Log(LogMessage message)
    {
        var severity = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information
        };

        logger.Log(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        return Task.CompletedTask;
    }

    // <summary>
    // Botが起動した際に実行されるイベント
    // </summary>
    public Task Ready()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                // ApplicationCommands の登録をここで行う。
                await interactions.RegisterCommandsToGuildAsync(ulong.Parse(configuration["DiscordBot:GuildId"] ?? ""));
                await interactions.RegisterCommandsToGuildAsync(ulong.Parse(configuration["DiscordBot:GuildId2"] ?? ""));
                await interactions.RegisterCommandsToGuildAsync(ulong.Parse(configuration["DiscordBot:GuildId3"] ?? ""));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Message}", ex.Message);
            }
        });

        return Task.CompletedTask;
    }

    // <summary>
    // 常時起動しているBotのイベント
    // </summary>
    private Task EventHandler(SocketGuildUser user)
    {
        _ = Task.Run(async () =>
        {
            await new GuildService().WelcomeMessageAsync(user);
        });

        return Task.CompletedTask;
    }

    // <summary>
    // 日付を越えてユーザーごとに一番最初のメッセージにリアクションをつける。
    // </summary>
    public Task MessageReceivedAsync(SocketMessage message)
    {
        _ = Task.Run(async () =>
        {
            // システムメッセージは無視
            if (message.Type != MessageType.Default)
                return;

            // Bot自身のメッセージも無視
            if (message.Author.IsBot) return;

            var userId = message.Author.Id;
            var today = DateTime.Now.Date;

            if (_userReactionDates.TryGetValue(userId, out DateTime lastReactedDate))
            {
                // すでに今日リアクション済みなら何もしない
                if (lastReactedDate == today)
                    return;
            }

            // リアクションを追加
            var emoji = Emote.Parse("<:good_cat:1371429892503240755>");
            await message.AddReactionAsync(emoji);

            // ユーザーのリアクション記録を更新
            _userReactionDates[userId] = today;
        });

        return Task.CompletedTask;
    }

    // <summary>
    // 毎日00:00に実行されるユーザーのリアクション記録の初期化
    // </summary>
    private void InitializeDailyResetTimer()
    {
        _ = Task.Run(async () =>
        {
            DateTime now = DateTime.Now;
            DateTime nextMidnight = now.Date.AddDays(1); // 翌日0時
            TimeSpan initialDelay = nextMidnight - now;

            // 以降は毎日24時間間隔でリセット
            _dailyResetTimer = new Timer(ResetDailyReactions, null, initialDelay, TimeSpan.FromDays(1));
        });
    }

    // <summary>
    // 初期化後のログ
    // </summary>
    private void ResetDailyReactions(object state)
    {
        _ = Task.Run(async () =>
        {
            Console.WriteLine($"[{DateTime.Now}] ユーザーのリアクション記録をリセットしました。");
            _userReactionDates.Clear();
        });
    }

    // <summary>
    // 挨拶のメッセージを送信する(新規参加者・システムチャンネル限定)
    // </summary>
    private Task SendGreetingMessage(SocketMessage message)
    {
        _ = Task.Run(async () =>
        {
            // Bot自身のメッセージは無視
            if (message.Author.IsBot) return;

            // メッセージに「よろしく」が含まれているかチェック
            if (!message.Content.Contains("よろしく", StringComparison.OrdinalIgnoreCase)) return;

            // ユーザーがSocketGuildUserであるか確認
            if (message.Author is not SocketGuildUser guildUser) return;

            // システムチャンネルかどうか確認
            var guild = guildUser.Guild;
            var systemChannel = guild.SystemChannel;
            if (systemChannel == null || message.Channel.Id != systemChannel.Id)
            {
                return; // システムチャンネル以外なら無視
            }

            // 入室から5分以内であるか確認
            var joinedAt = guildUser.JoinedAt;
            if (joinedAt == null) return; // 加入時刻が取得できない場合は無視

            // 現在時刻との差分が5分以内か判定
            var timeSinceJoin = DateTimeOffset.UtcNow - joinedAt.Value;
            if (timeSinceJoin > TimeSpan.FromMinutes(5)) return;
            {
                await message.Channel.SendMessageAsync("よろしくお願いします～！<:good_cat:1371429892503240755>");
            }
           
        });

        return Task.CompletedTask;
    }
}
