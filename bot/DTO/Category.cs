using System.Text.Json.Serialization;

namespace bot.DTO;
public class Category
{
    [JsonPropertyName("CategoryId")]
    public string CategoryId { get; set; }

    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Items")]
    public List<Item> Items { get; set; }
}