namespace bot.BotServices;
public class MessageBuilders
{
    public static string ItemsMessage(List<string> items)
    {
        var str = "Here our drinks list. Here you can find anything you want.\n";
        foreach (var item in items)
        {
            str += $"{items.IndexOf(item) + 1}. {item}.\n";
        }
        return str;
    }
}