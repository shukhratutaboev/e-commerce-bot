using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bot.Entities;
public class Book
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();
    public long ClientChatId { get; set; }
    public DateTime BookedTime { get; set; }
    public long Total { get; set; }
    public int BookNumber { get; set; }
}