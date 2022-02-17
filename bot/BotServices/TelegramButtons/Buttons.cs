using Telegram.Bot.Types.ReplyMarkups;

namespace bot.BotServices.TelegramButtons;
public class Buttons
{
    public static IReplyMarkup Menu()
        => new ReplyKeyboardMarkup(new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {
                new KeyboardButton("Book"){},
                new KeyboardButton("History"){}
            },
            new List<KeyboardButton>()
            {
                new KeyboardButton("Locations"){},
                new KeyboardButton("Settings"){}
            }
        })
        {
            ResizeKeyboard =true,
            OneTimeKeyboard = true
        };
    public static IReplyMarkup Categories(List<string> elements)
    {
        var buttons = new List<List<KeyboardButton>>(){};

        for (var i = 0; i < elements.Count/2; i++)
        {
            buttons.Add(
                new List<KeyboardButton>()
                {
                    new KeyboardButton(elements[i*2]),
                    new KeyboardButton(elements[i*2+1])
                }
            );
        }

        if(elements.Count % 2 != 0)
        {
            buttons.Add(
                new List<KeyboardButton>()
                {
                    new KeyboardButton(elements.Last()),
                }
            );
        }

        buttons.Add(
            new List<KeyboardButton>()
            {
                new KeyboardButton("Cart"),
                new KeyboardButton("Back to menu")
            }
        );

        return new ReplyKeyboardMarkup(buttons)
            {
                ResizeKeyboard =true,
                OneTimeKeyboard = true
            };
    }
}