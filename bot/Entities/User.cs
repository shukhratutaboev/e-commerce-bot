using System.ComponentModel.DataAnnotations;

namespace bot.Entities;
public class User
{
    [Key]
    public long? ChatId { get; set; }
    public string? Username { get; set; }
    public string? Fullname { get; set; }
    public string? PhoneNumber { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string? Address { get; set; }
    public Process Process { get; set; }
    
    [Obsolete("Used only for Entity binding.")]
    public User(){ }
    public User(long chatId, string username)
    {
        ChatId = chatId;
        Username = username == "Empty" ? "Empty" : $"@{username}";
        Fullname = string.Empty;
        PhoneNumber = string.Empty;
        Longitude = 0;
        Latitude = 0;
        Address = string.Empty;
        Process = Process.None;
    }
}