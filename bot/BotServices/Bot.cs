using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace bot.BotServices;
public class Bot : BackgroundService
{
    private readonly TelegramBotClient _client;
    private readonly ILogger<Bot> _logger;

    public Bot(TelegramBotClient client, ILogger<Bot> logger, BotHandlers handlers)
    {
        _client = client;
        _logger = logger;
        _client.StartReceiving(new DefaultUpdateHandler(handlers.HandleUpdateAsync, handlers.HandleErrorAsync));
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var me = await _client.GetMeAsync();
        _logger.LogInformation($"{me.Username} has connected successfully");
    }
}