using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace dashboard.Entities;
public class Category
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ICollection<Item> Items { get; set; }

    [Obsolete("Only for entity binding.")]
    public Category(){}
    public Category(string name, ICollection<Item> items = default)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}