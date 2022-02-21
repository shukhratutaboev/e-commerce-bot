using System.ComponentModel.DataAnnotations;
namespace dashboard.Models;
public class NewCategory
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
}