using System.ComponentModel.DataAnnotations;
namespace dashboard.Models;
public class NewItem
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
    
    [Required]
    public double Cost { get; set; }

    // [Required]
    [Display(Name="File")]
    public IFormFile ImageUrl { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
}