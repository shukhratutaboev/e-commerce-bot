using bot.DTO;

namespace bot.BotServices;
public class MessageBuilders
{
    public static string ItemsMessage(List<Item> items)
    {
        var str = $"Category {items[0].Category.Name}:\n";
        foreach (var item in items)
        {
            str += $"{items.IndexOf(item) + 1}. {item.Name} {item.Cost} so'm.\n";
        }
        return str;
    }
}