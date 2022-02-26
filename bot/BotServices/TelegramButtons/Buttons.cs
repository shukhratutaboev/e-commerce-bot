using Telegram.Bot.Types.ReplyMarkups;

namespace bot.BotServices.TelegramButtons;
public class Buttons
{
    public static IReplyMarkup SendLocation()
        => new ReplyKeyboardMarkup(new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {
                new KeyboardButton("📍 Lokatsiya jo'natish"){ RequestLocation = true}
            }
        })
        {
            ResizeKeyboard =true,
            OneTimeKeyboard = true
        };
    public static IReplyMarkup SendContact()
        => new ReplyKeyboardMarkup(new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {
                new KeyboardButton("📞 Kontaktni jo'natish"){ RequestContact = true}
            }
        })
        {
            ResizeKeyboard =true,
            OneTimeKeyboard = true
        };
    public static IReplyMarkup Menu()
        => new ReplyKeyboardMarkup(new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {
                new KeyboardButton("🍽 Menyu"){}
                //new KeyboardButton("Savatcha"){}
            },
            new List<KeyboardButton>()
            {
                new KeyboardButton("☎️ Kontaktimiz"){},
                new KeyboardButton("⚙️ Sozlamalar"){}
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
                new KeyboardButton("🛒 Savatcha"),
                new KeyboardButton("↪️ Orqaga")
            }
        );

        return new ReplyKeyboardMarkup(buttons)
            {
                ResizeKeyboard =true,
                OneTimeKeyboard = true
            };
    }
}