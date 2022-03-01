using bot.Entities;

namespace bot.Services;
public interface IStorageService
{
    Task<(bool IsSuccess, Exception exception)> InsertUserAsync(User user);
    Task<(bool IsSuccess, Exception exception)> UpdateUserAsync(User user);
    Task<bool> ExistsAsync(long? chatId);
    Task<(User user, bool IsSuccess, Exception exception)> GetUserAsync(long? chatId);
    Task<(List<User> Users, bool IsSuccess, Exception exception)> GetUsersAsync();
    int GetNumber();
    Task<Book> CreateBookAsync(long? chatId, long total);
}