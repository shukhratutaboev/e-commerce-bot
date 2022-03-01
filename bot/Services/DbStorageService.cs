using bot.Entities;
using Microsoft.EntityFrameworkCore;

namespace bot.Services;
public class DbStorageService : IStorageService
{
#pragma warning disable
    private readonly BotDbContext _ctx;
    private readonly ILogger<DbStorageService> _logger;

    public DbStorageService(BotDbContext ctx, ILogger<DbStorageService> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }
    public async Task<Book> CreateBookAsync(long? chatId, long total)
    {
        var book = new Book()
        {
            BookedTime = DateTime.Now,
            ClientChatId = chatId??1,
            Total = total,
            BookNumber = GetNumber(),
        };
        await _ctx.Books.AddAsync(book);
        await _ctx.SaveChangesAsync();
        return book;
    }
    public int GetNumber()
    => _ctx.Books.Select(b => b.BookedTime.ToShortDateString() == DateTime.Now.ToShortDateString()).ToListAsync().Result.Count%100;
    
    public async Task<bool> ExistsAsync(long? chatId)
        => await _ctx.Users.AnyAsync(u => u.ChatId == chatId);

    public async Task<(User user, bool IsSuccess, Exception exception)> GetUserAsync(long? chatId)
    {
        try
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
            if(user == default) return (null, false, new Exception("User doesn't exist"));
            else return (user, true, null);
        }
        catch(Exception e)
        {
            return (null, false, e);
        }
    }

    public async Task<(List<User> Users, bool IsSuccess, Exception exception)> GetUsersAsync()
    {
        try
        {
            var users = await _ctx.Users.ToListAsync();
            return (users, true, null);
        }
        catch(Exception e)
        {
            return (null, false, e);
        }
    }

    public async Task<(bool IsSuccess, Exception exception)> InsertUserAsync(User user)
    {
        try
        {
            if(await ExistsAsync(user.ChatId))
            {
                _logger.LogInformation($"User already exists: {user.ChatId} {user.Username}");
                return (false, new Exception($"User already exists: {user.ChatId} {user.Username}"));
            }
            await _ctx.Users.AddAsync(user);
            await _ctx.SaveChangesAsync();
            _logger.LogInformation($"Added new user to DB: {user.ChatId} {user.Username}");
            return (true, null);
        }
        catch(Exception e)
        {
            _logger.LogError($"Can't add user to DB. Error:\n{e.Message}");
            return (false, e);
        }
    }

    public async Task<(bool IsSuccess, Exception exception)> UpdateUserAsync(User user)
    {
        try
        {
            _ctx.Users.Update(user);
            await _ctx.SaveChangesAsync();
            _logger.LogInformation($"User is updated: {user.ChatId} {user.Username}");
            return (true, null);
        }
        catch(Exception e)
        {
            _logger.LogError($"Can't update user. Error:\n{e.Message}");
            return (false, e);
        }
    }
}