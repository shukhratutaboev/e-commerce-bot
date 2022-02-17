using bot.BotServices.TelegramButtons;
using bot.Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace bot.BotServices
{
#pragma warning disable
    public class BotHandlers
    {
        private readonly ILogger<BotHandlers> _logger;
        private readonly IStorageService _storage;

        public BotHandlers(ILogger<BotHandlers> logger, IStorageService storage)
        {
            _logger = logger;
            _storage = storage;
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
                UpdateType.CallbackQuery => BotOnCallbackQueryRecieved(client, update.CallbackQuery),
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

        private async Task BotOnCallbackQueryRecieved(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            if(callbackQuery.Data == "plus")
            {
                await client.EditMessageTextAsync(
                    callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId,
                    $"{int.Parse(callbackQuery.Message.Text) + 10}",
                    replyMarkup: InlineButtons.CartItem()
                );
            }
            if(callbackQuery.Data == "minus")
            {
                await client.EditMessageTextAsync(
                    callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId,
                    $"{int.Parse(callbackQuery.Message.Text) - 5}",
                    replyMarkup: InlineButtons.CartItem()
                );
            }
        }

        private Task UnknownUpdateHandler(ITelegramBotClient client, Update update)
        {
            _logger.LogWarning("This type update can't be handled.");
            return Task.CompletedTask;
        }

        private async Task BotOnMessageRecieved(ITelegramBotClient client, Message? message)
        {
            if(message.Text == "/start")
            {
                await _storage.InsertUserAsync(new Entities.User(message.Chat.Id, message.From.Username??"Empty"));
                await client.SendTextMessageAsync(
                    message.Chat.Id,
                    "Welcome)",
                    replyMarkup: Buttons.Menu()
                );
            }
            if(message.Text == "Book")
            {
                await client.SendTextMessageAsync(
                    message.Chat.Id,
                    "Categories",
                    replyMarkup: Buttons.Categories(new List<string>(){"Pizzas", "Lavashes", "Drinks", "HotDogs", "Sets"})
                );
            }
            if(message.Text == "Drinks")
            {
                await client.SendTextMessageAsync(
                    message.Chat.Id,
                    MessageBuilders.ItemsMessage(new List<string>(){"CocaCola", "Pepsi", "Fanta", "Bliss", "Lemon-Tea", "Coffee"}),
                    replyMarkup: InlineButtons.Items(new List<string>(){"CocaCola", "Pepsi", "Fanta", "Bliss", "Lemon-Tea", "Coffee"})
                );
            }
            if(message.Text == "Cart")
            {
                await client.SendTextMessageAsync(
                    message.Chat.Id,
                    "1",
                    replyMarkup: InlineButtons.CartItem()
                );
            }
        }
    }
}