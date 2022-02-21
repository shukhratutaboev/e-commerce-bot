using dashboard.Entities;
using dashboard.Models;

namespace dashboard.Mappers;
public static class ModelEntityMappers
{
    public static NewItem ToModel(this Item item)
        => new NewItem(){
            Name = item.Name,
            Cost = item.Cost,
            ImageUrl = default,
            CategoryId = item.CategoryId
        };
    public static Category ToEntity(this NewCategory category)
        => new Category(category.Name);
    public static Item ToEntity(this NewItem item, Category category)
        => new Item(){
            ItemId = Guid.NewGuid(),
            Name = item.Name,
            Cost = item.Cost,
            ImageUrl =  item.ImageUrl.toByte(),
            CategoryId = item.CategoryId,
            Category = category
        };
    public static string toByte(this IFormFile image)
    {
        var memoryStream = new MemoryStream();
        image.CopyToAsync(memoryStream);
        var result = memoryStream.ToArray();
        while(result.Count() == 0) result = memoryStream.ToArray();
        var str = Convert.ToBase64String(result);
        return "data:data:image/jpeg;base64,"+str;
    }
}