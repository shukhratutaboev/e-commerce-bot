using System.Text.RegularExpressions;
using System.Diagnostics;
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
                    $"{int.Parse(callbackQuery.Message.Text) + 1}",
                    replyMarkup: InlineButtons.CartItem()
                );
            }
            if(callbackQuery.Data == "minus")
            {
                await client.EditMessageTextAsync(
                    callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId,
                    $"{int.Parse(callbackQuery.Message.Text) - 1}",
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
            if(message.Text == "/start" && !_storage.ExistsAsync(message.Chat.Id).Result)
            {
                await _storage.InsertUserAsync(new Entities.User(message.Chat.Id, message.From.Username));
                await client.SendTextMessageAsync(
                    message.Chat.Id,
                    "<b>Welcome.</b>\n\nPlease enter your fullname:",
                    parseMode: ParseMode.Html
                );
            }
            else
            {
                var user = (await _storage.GetUserAsync(message.Chat.Id)).user;
                if(user.Process != Entities.Process.None)
                {
                    if(user.Process == Entities.Process.EnteringFullName)
                    {
                        user.Fullname = message.Text;
                        user.Process = Entities.Process.SendingContact;
                        await _storage.UpdateUserAsync(user);
                        await client.SendTextMessageAsync(
                            user.ChatId,
                            "Please send me your contact or own phone number (ex: +998912345678):\n",
                            replyMarkup: Buttons.SendContact()
                        );
                    }
                    else if(user.Process == Entities.Process.SendingContact)
                    {
                        if(message.Contact is null)
                        {
                            if(Regex.Match(message.Text, @"(?:[+][9]{2}[8][0-9]{2}[0-9]{3}[0-9]{2}[0-9]{2})").Success)
                            {
                                user.PhoneNumber = message.Text;
                            }
                            else
                            {
                                await client.SendTextMessageAsync(
                                    user.ChatId,
                                    "Message must be contact or phone number (ex: +998912345678)!"
                                );
                                _logger.LogInformation($"Number doesn't match: {user.ChatId} {user.Username}");
                                return;
                            }
                        }
                        else user.PhoneNumber = "+"+message.Contact.PhoneNumber;
                        user.Process = Entities.Process.SendingLocation;
                        await _storage.UpdateUserAsync(user);
                        await client.SendTextMessageAsync(
                            user.ChatId,
                            "Menu",
                            replyMarkup: Buttons.Menu()
                        );
                    }
                }
                // else if(user.Process == Entities.Process.SendingLocation)
                // {

                //     user.Latitude = message.Location.Latitude;
                //     user.Longitude = message.Location.Longitude;
                //     user.Process = Entities.Process.None;
                //     await _storage.UpdateUserAsync(user);
                //     await client.SendTextMessageAsync(
                //         user.ChatId,
                //         "Now you can use this bot."
                //     );
                // }
            }
        }
    }
}