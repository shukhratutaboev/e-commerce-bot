using System.Text.RegularExpressions;
using bot.BotServices.TelegramButtons;
using bot.Entities;
using bot.Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace bot.BotServices;
#pragma warning disable
public class BotHandlers
{
    private readonly ILogger<BotHandlers> _logger;
    private readonly IStorageService _storage;
    private readonly DashboardClient _client;
    private readonly CartInternalService _cartService;

    public BotHandlers(ILogger<BotHandlers> logger, IStorageService storage, DashboardClient client, CartInternalService cartService)
    {
        _logger = logger;
        _storage = storage;
        _client = client;
        _cartService = cartService;
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
        if (callbackQuery.Data.Split(" ").ToArray()[0] == "add")
        {
            var a = int.Parse(callbackQuery.Data.Split(" ").ToArray()[1]);
            var id = callbackQuery.Data.Split(" ").ToArray()[2];
            await client.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                "Mahsulot savatchaga qo'shildi✅", 
                false
            );
            await client.DeleteMessageAsync(
                callbackQuery.Message.Chat.Id,
                callbackQuery.Message.MessageId
            );
            _cartService.UpdateCart(callbackQuery.Message.Chat.Id, id, a);
            var cart = _cartService.GetUserCart(callbackQuery.Message.Chat.Id);
            var text = "";
            if (cart.Cart.Count == 0) text = "Savatcha bo'sh";
            else
            {
                long sum = 0;
                var index = 1;
                foreach (var i in cart.Cart)
                {
                    var item = _client.GetItemAsync(i.ItemId).Result.item;
                    sum += i.Quantity * (long)item.Cost;
                    text += $"\n{index++}) <b>{item.Name}</b>:\n    {i.Quantity} x {item.Cost} = {i.Quantity * item.Cost} so'm";
                }
                text += $"\n\nJami: <b>{sum.ToString("# ##0")} so'm</b>";
            }
            try
            {
                await client.EditMessageTextAsync(
                    callbackQuery.Message.Chat.Id,
                    cart.MessageId,
                    text,
                    ParseMode.Html,
                    replyMarkup: cart.Cart.Count == 0 ? null : InlineButtons.Cart()
                );
            }
            catch { }
        }
        if (_client.GetItemsByCategoryAsync("all").Result.items.Any(i => i.ItemId == callbackQuery.Data))
        {
            var item = _client.GetItemAsync(callbackQuery.Data).Result.item;
            byte[] data = System.Convert.FromBase64String(item.ImageUrl.Replace("data:data:image/jpeg;base64,/", "/").Replace("data:image/jpeg;base64,/", "/"));
            MemoryStream ms = new MemoryStream(data);
            await client.SendPhotoAsync(
                callbackQuery.Message.Chat.Id,
                ms,
                $"<b>{item.Name}</b>\n1 x {item.Cost} = {item.Cost} so'm",
                ParseMode.Html,
                replyMarkup: InlineButtons.CartItem(1, item.ItemId)
            );
        }
        if(callbackQuery.Data == "clear")
        {
            _cartService.ClearCart(callbackQuery.Message.Chat.Id);
            await client.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
        }
        if(callbackQuery.Data == "book")
        {
            var user = _storage.GetUserAsync(callbackQuery.Message.Chat.Id).Result.user;
            if(user.Address == string.Empty)
            {
                await client.DeleteMessageAsync(user.ChatId, callbackQuery.Message.MessageId); 
                await client.SendTextMessageAsync(
                    user.ChatId,
                    "Iltimos lokatsiyangizni jo'nating",
                    replyMarkup: Buttons.SendLocation()
                );
                user.Process = Entities.Process.SendingLocation;
                await _storage.UpdateUserAsync(user);
            }
            else await client.EditMessageTextAsync(
                    callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId,
                    $"Ma'lumotlaringizni tekshirib olganingiz maqul😊\n\nTelefon raqam: {user.PhoneNumber}.\nManzil: {user.Address}.",
                    ParseMode.Html,
                    replyMarkup: InlineButtons.IsRightNumber());
        }
        if(callbackQuery.Data == "edit")
        {
            await client.DeleteMessageAsync(
                callbackQuery.Message.Chat.Id,
                callbackQuery.Message.MessageId
            );
            await client.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                "Iltimos kontaktingizni yoki telefon raqamingizni jo'nating.",
                replyMarkup: Buttons.SendContact());
            var user = _storage.GetUserAsync(callbackQuery.Message.Chat.Id).Result.user;
            user.Process = Entities.Process.SendingContact;
            await _storage.UpdateUserAsync(user);
        }
        if(callbackQuery.Data.Split(" ").ToArray()[0] == "checked")
        {
            await client.EditMessageTextAsync(
                callbackQuery.Message.Chat.Id,
                callbackQuery.Message.MessageId,
                callbackQuery.Message.Text + "\n\n✅ Tekshirildi");
        }
        if (callbackQuery.Data == "canceled")
        {
            await client.EditMessageTextAsync(
                callbackQuery.Message.Chat.Id,
                callbackQuery.Message.MessageId,
                callbackQuery.Message.Text + "\n\n❌ Bekor qilindi");
        }
        if(callbackQuery.Data == "delete")
        {
            await client.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
        }
        if (callbackQuery.Data == "right")
        {
            await client.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                "Buyurtmangiz qabul qilindi",
                replyMarkup: Buttons.Menu());
            await client.DeleteMessageAsync(
                callbackQuery.Message.Chat.Id,
                callbackQuery.Message.MessageId);
            await SendToAdmin(client, _cartService.GetUserCart(callbackQuery.Message.Chat.Id));
            _cartService.ClearCart(callbackQuery.Message.Chat.Id);
        }
        if(callbackQuery.Data.Split(" ").ToArray()[0] == "+")
        {
            var a = int.Parse(callbackQuery.Data.Split(" ").ToArray()[1]) + 1;
            var id = callbackQuery.Data.Split(" ").ToArray()[2];
            var item = _client.GetItemAsync(id).Result.item;
            await client.EditMessageCaptionAsync(
                callbackQuery.Message.Chat.Id,
                callbackQuery.Message.MessageId,
                $"<b>{item.Name}</b>\n{a} x {item.Cost} = {item.Cost * a} so'm",
                ParseMode.Html,
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
                $"<b>{item.Name}</b>\n{a} x {item.Cost} = {item.Cost * a} so'm",
                ParseMode.Html,
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
                "Salom\n\nIltimos ismingizni kiriting:",
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
                        $"{user.Fullname}, iltimos kontaktingizni yoki telefon raqamingizni jo'nating (misol: +998912345678):\n",
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
                                "Telefon raqam quyidagidek formatda bo'lishi kerak \"+998912345678\""
                            );
                            _logger.LogInformation($"Number doesn't match: {user.ChatId} {user.Username}");
                            return;
                        }
                    }
                    else user.PhoneNumber = message.Contact.PhoneNumber;
                    user.Process = Entities.Process.None;
                    await _storage.UpdateUserAsync(user);
                    if (_cartService.GetUserCart(message.Chat.Id) is null || _cartService.GetUserCart(message.Chat.Id).Cart.Count == 0)
                    await client.SendTextMessageAsync(
                        user.ChatId,
                        "Botdan foydalanishingiz mumkin 😊",
                        replyMarkup: Buttons.Menu()
                    );
                    else
                    {
                        await client.SendTextMessageAsync(
                            user.ChatId,
                            "Endi lokatsiyangizni jo'nating",
                            replyMarkup: Buttons.SendLocation()
                        );
                        user.Process = Entities.Process.SendingLocation;
                        await _storage.UpdateUserAsync(user);
                    }
                }
                else if(user.Process == Entities.Process.SendingLocation)
                {
                    if(message.Location is null)
                    {
                        await client.SendTextMessageAsync(
                            user.ChatId,
                            "Iltimos lokatsiyangizni jo'nating"
                        );
                    }
                    else
                    {
                        user.Longitude = message.Location.Longitude;
                        user.Latitude = message.Location.Latitude;
                        user.Process = Entities.Process.EnteringAddress;
                        await _storage.UpdateUserAsync(user);
                        await client.SendTextMessageAsync(
                            user.ChatId,
                            "Iltimos lokatsiyaga qo'shimcha ravishda manzilingizni kiritsangiz. Masalan mahalla, uy va kvartira raqami."
                        );
                    }
                }
                else if(user.Process == Entities.Process.EnteringAddress)
                {
                    if(message.Text is null)
                    {
                        await client.SendTextMessageAsync(
                            user.ChatId,
                            "Iltimos manzilni tekst ko'rinishida jo'nating"
                        );
                    }
                    else
                    {
                        user.Address = message.Text;
                        await _storage.UpdateUserAsync(user);
                        await client.SendTextMessageAsync(
                            user.ChatId,
                            $"Ma'lumotlaringizni tekshirib olganingiz maqul😊\n\nTelefon raqam: {user.PhoneNumber}.\nManzil: {user.Address}.",
                            ParseMode.Html,
                            replyMarkup: InlineButtons.IsRightNumber());
                        user.Process = Entities.Process.None;
                        await _storage.UpdateUserAsync(user);
                    }
                }
            }
            else
            {
                if(message.Text == "🍽 Menyu")
                {
                    await client.SendTextMessageAsync(
                        message.Chat.Id,
                        "Menyu",
                        replyMarkup: Buttons.Categories(_client.GetCategoriesAsync().Result.categories.Select(c => c.Name).ToList())
                    );
                }
                if(message.Text == "🛒 Savatcha")
                {
                    var cart = _cartService.GetUserCart(message.Chat.Id);
                    var text = "";
                    if (cart is null || cart.Cart.Count == 0) text = "Savatcha bo'sh";
                    else
                    {
                        long sum = 0;
                        var index = 1;
                        foreach (var i in cart.Cart)
                        {
                            var item = _client.GetItemAsync(i.ItemId).Result.item;
                            sum += i.Quantity * (long)item.Cost;
                            text += $"\n{index++}) <b>{item.Name}</b>:\n    {i.Quantity} x {item.Cost} = {i.Quantity * item.Cost} so'm";
                        }
                        text += $"\n\nJami: <b>{sum.ToString("# ##0")} so'm</b>";
                    }
                    var m = await client.SendTextMessageAsync(
                        user.ChatId,
                        text,
                        ParseMode.Html,
                        replyMarkup: cart is null || cart.Cart.Count == 0 ? null : InlineButtons.Cart()
                    );
                    cart = cart is null ? new UserCart(message.Chat.Id) : cart;
                    cart.MessageId = m.MessageId;
                    _cartService.UpdateCart(cart);
                }
                if(message.Text == "↪️ Orqaga")
                {
                    await client.SendTextMessageAsync(
                        user.ChatId,
                        "Asosiy panel",
                        replyMarkup: Buttons.Menu()
                    );
                }
                if(_client.GetCategoriesAsync().Result.categories.Select(c => c.Name).ToList().Contains(message.Text))
                {
                    var items = _client.GetItemsByCategoryAsync(message.Text).Result.items;
                    await client.SendTextMessageAsync(
                        message.Chat.Id,
                        MessageBuilders.ItemsMessage(items),
                        ParseMode.Html,
                        replyMarkup: InlineButtons.Items(items)
                    );
                }
            }
        }
    }
    public async Task SendToAdmin(ITelegramBotClient client, UserCart cart)
    {
        var admin = 426131983;
        var user = _storage.GetUserAsync(cart.UserId).Result.user;
        var text = $"Yangi buyurtma:\n\nFoydalanuvchi: {user.Fullname}\nTelefon raqam: {user.PhoneNumber}\nManzil: {user.Address}\nBuyurtma:";
        foreach(var i in cart.Cart)
        {
            var item = _client.GetItemAsync(i.ItemId).Result.item;
            text += $"\n    {cart.Cart.IndexOf(i) + 1}) {item.Name} - {i.Quantity} ta";
        }
        var m =await client.SendTextMessageAsync(
            admin,
            text,
            replyMarkup: InlineButtons.AdminChecking(user.ChatId));
        await client.SendLocationAsync(admin,user.Latitude, user.Longitude,replyToMessageId: m.MessageId);
    }
}