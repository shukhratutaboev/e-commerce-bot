using System.Text.RegularExpressions;
using System.Diagnostics;
using bot.BotServices.TelegramButtons;
using bot.Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace bot.BotServices
{
#pragma warning disable
    public class BotHandlers
    {
        private readonly ILogger<BotHandlers> _logger;
        private readonly IStorageService _storage;
        private readonly DashboardClient _client;

        public BotHandlers(ILogger<BotHandlers> logger, IStorageService storage, DashboardClient client)
        {
            _logger = logger;
            _storage = storage;
            _client = client;
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
            if(_client.GetItemsByCategoryAsync("all").Result.items.Any(i => i.ItemId == callbackQuery.Data))
            {
                var item = _client.GetItemAsync(callbackQuery.Data).Result.item;
                byte[] data = System.Convert.FromBase64String(item.ImageUrl.Replace("data:data:image/jpeg;base64,/", "/"));
                MemoryStream ms = new MemoryStream(data);
                await client.SendPhotoAsync(
                    callbackQuery.Message.Chat.Id,
                    ms,
                    $"1 x {item.Name} {item.Cost} so'm",
                    ParseMode.Markdown,
                    replyMarkup: InlineButtons.CartItem(1, item.ItemId)
                );
            }
            if(callbackQuery.Data.Split(" ").ToArray()[0] == "+")
            {
                var a = int.Parse(callbackQuery.Data.Split(" ").ToArray()[1]) + 1;
                var id = callbackQuery.Data.Split(" ").ToArray()[2];
                var item = _client.GetItemAsync(id).Result.item;
                await client.EditMessageCaptionAsync(
                    callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId,
                    $"{a} x {item.Name} {item.Cost * a} so'm",
                    replyMarkup: InlineButtons.CartItem(a, id)
                );
            }
            if(callbackQuery.Data.Split(" ").ToArray()[0] == "-")
            {
                var a = int.Parse(callbackQuery.Data.Split(" ").ToArray()[1]) - 1;
                a = a < 0 ? 0 : a; 
                var id = callbackQuery.Data.Split(" ").ToArray()[2];
                var item = _client.GetItemAsync(id).Result.item;
                await client.EditMessageCaptionAsync(
                    callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId,
                    $"{a} x {item.Name} {item.Cost * a} so'm",
                    replyMarkup: InlineButtons.CartItem(a, id)
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
                        user.Process = Entities.Process.None;
                        await _storage.UpdateUserAsync(user);
                        await client.SendTextMessageAsync(
                            user.ChatId,
                            "Menu",
                            replyMarkup: Buttons.Menu()
                        );
                    }
                }
                else
                {
                    if(message.Text == "Book")
                    {
                        await client.SendTextMessageAsync(
                            message.Chat.Id,
                            "Categories",
                            replyMarkup: Buttons.Categories(_client.GetCategoriesAsync().Result.categories.Select(c => c.Name).ToList())
                        );
                    }
                    if(_client.GetCategoriesAsync().Result.categories.Select(c => c.Name).ToList().Contains(message.Text))
                    {
                        var items = _client.GetItemsByCategoryAsync(message.Text).Result.items;
                        // foreach(var i in items)
                        // {
                        //     try
                        //     {
                        //         byte[] data = System.Convert.FromBase64String(i.ImageUrl.Replace("data:data:image/jpeg;base64,/", "/"));
                        //         MemoryStream ms = new MemoryStream(data);
                        //         await client.SendPhotoAsync(
                        //             message.Chat.Id,
                        //             ms,
                        //             $"0x{i.Name} 0 so'm",
                        //             ParseMode.Markdown,
                        //             replyMarkup: InlineButtons.CartItem(1, i.ItemId)
                        //         );
                        //     }
                        //     catch(Exception e){}
                        // }
                        await client.SendTextMessageAsync(
                            message.Chat.Id,
                            MessageBuilders.ItemsMessage(items),
                            replyMarkup: InlineButtons.Items(items)
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