using Telegram.Bot.Types.ReplyMarkups;

namespace bot.BotServices.TelegramButtons;
public class InlineButtons
{
    public static InlineKeyboardMarkup CartItem()
        => new InlineKeyboardMarkup(
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(text: "➖", "minus"),
                InlineKeyboardButton.WithCallbackData(text: "➕", "plus"),
            }
        );
    public static IReplyMarkup Items(List<string> elements)
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
                                                    callbackData:elements[i * count / 2 + j])
                    );
                }
            }

            if(count % 2 != 0)
            {
                buttons[1].Add(
                    InlineKeyboardButton.WithCallbackData(text:$"{count}", callbackData: elements.Last())
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
                                                callbackData:item)
                );
            }
        }
        buttons.Add(
            new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData(text:"⬅️", callbackData:"prev"),
                InlineKeyboardButton.WithCallbackData(text:"➡️", callbackData:"next")
            }
        );
        return new InlineKeyboardMarkup(buttons);
    }
}