using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace dashboard.Entities;
public class Category
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid CategoryId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual ICollection<Item> Items { get; set; }

    [Obsolete("Only for entity binding.")]
    public Category(){}
    public Category(string name, ICollection<Item> items = default)
    {
        CategoryId = Guid.NewGuid();
        Name = name;
    }
}