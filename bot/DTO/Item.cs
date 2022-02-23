using System.Text.Json.Serialization;

namespace bot.DTO;
public class Item
{
    [JsonPropertyName("ItemId")]
    public string ItemId { get; set; }

    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Cost")]
    public double Cost { get; set; }

    [JsonPropertyName("ImageUrl")]
    public string ImageUrl { get; set; }

    [JsonPropertyName("CategoryId")]
    public string CategoryId { get; set; }

    [JsonPropertyName("Category")]
    public Category Category { get; set; }
}