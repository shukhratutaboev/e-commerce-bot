using bot.DTO;

namespace bot.BotServices;
public class MessageBuilders
{
    public static string ItemsMessage(List<Item> items)
    {
        var str = $"Kategoriya {items[0].Category.Name}:\n\n";
        foreach (var item in items)
        {
            str += $"{items.IndexOf(item) + 1}) <b>{item.Name}</b> {item.Cost} so'm.\n";
        }
        return str;
    }
}