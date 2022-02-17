using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace bot.Services
{
#pragma warning disable
    public class BotHandlers
    {
        private readonly ILogger<BotHandlers> _logger;

        public BotHandlers(ILogger<BotHandlers> logger)
        {
            _logger = logger;
        }
        public Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken ctoken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException => $"Error occured with Telegram Client: {exception.Message}",
                _ => exception.Message
            };
            _logger.LogCritical(errorMessage);
            return Task.CompletedTask;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken ctoken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageRecieved(client, update.Message),
                _ => UnknownUpdateHandler(client, update)
            };
            try
            {
                await handler;
            }
            catch(Exception e)
            {
                _logger.LogWarning(e.Message);
            }
        }

        private Task UnknownUpdateHandler(ITelegramBotClient client, Update update)
        {
            throw new Exception("This type update can't be handled.");
        }

        private async Task BotOnMessageRecieved(ITelegramBotClient client, Message? message)
        {
            if(message.Text == "/start")
            {
                await client.SendTextMessageAsync(
                    message.Chat.Id,
                    "Hello)"
                );
            }
        }
    }
}