using bot.DTO;
using Telegram.Bot.Types.ReplyMarkups;

namespace bot.BotServices.TelegramButtons;
public class InlineButtons
{
    public static InlineKeyboardMarkup AdminChecking(long? chatId)
        => new InlineKeyboardMarkup(
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(text: "✅ Tekshirildi", $"checked {chatId}"),
                InlineKeyboardButton.WithCallbackData(text: "❌ Bekor qilindi", "canceled")
            }
        );
    public static InlineKeyboardMarkup IsRightNumber()
        => new InlineKeyboardMarkup(
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(text: "✅ Tog'ri", "right"),
                InlineKeyboardButton.WithCallbackData(text: "✍️ Tahrirlash", "edit")
            }
        );
    // Savatchaga qo'shilib ishlatiladi
    public static InlineKeyboardMarkup Cart()
        => new InlineKeyboardMarkup(
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(text: "✅ Tasdiqlash", "book"),
                InlineKeyboardButton.WithCallbackData(text: "❌ Tozalash", "clear")
            }
        );

    // Item'larni miqdorini belgilash uchun, item'ga qo'shilib junatiladi
    public static InlineKeyboardMarkup CartItem(int a, string id)
    {
        var ar = new List<InlineKeyboardButton>();
        if(a != 0) ar.Add(InlineKeyboardButton.WithCallbackData(text: "📥 Savatchaga", $"add {a} {id}"));
        ar.Add(InlineKeyboardButton.WithCallbackData(text: "❌", "delete"));
        var ik = new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>()
        {
            new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData(text: "➖", $"- {a} {id}"),
                $"{a}",
                InlineKeyboardButton.WithCallbackData(text: "➕", $"+ {a} {id}"),
            },
            ar
        });
        return ik;
    }

    // Bu buttonlar category'dagi item'larni raqamlab beradi
    public static IReplyMarkup Items(List<Item> elements)
    {
        var buttons = new List<List<InlineKeyboardButton>>(){};
        var count = elements.Count;

        if(count > 4)
        {
            for (var i = 0; i < 2; i++)
            {
                buttons.Add(new List<InlineKeyboardButton>());
                for (var j = 0; j < count/2; j++)
                {
                    buttons[i].Add(
                        InlineKeyboardButton.WithCallbackData(text:$"{i * count / 2 + j + 1}",
                                                    callbackData:elements[i * count / 2 + j].ItemId)
                    );
                }
            }

            if(count % 2 != 0)
            {
                buttons[1].Add(
                    InlineKeyboardButton.WithCallbackData(text:$"{count}", callbackData: elements.Last().ItemId)
                );
            }
        }
        else
        {
            buttons.Add(new List<InlineKeyboardButton>());
            foreach (var item in elements)
            {
                buttons[0].Add(
                    InlineKeyboardButton.WithCallbackData(text:$"{elements.IndexOf(item) + 1}",
                                                callbackData:item.ItemId)
                );
            }
        }
        return new InlineKeyboardMarkup(buttons);
    }
}