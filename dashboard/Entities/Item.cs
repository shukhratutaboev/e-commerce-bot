using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dashboard.Entities;
public class Item
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid ItemId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    [Required]
    public double Cost { get; set; }
    public string ImageUrl { get; set; }
    public Guid CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; }
}