using dashboard.Entities;
using Microsoft.EntityFrameworkCore;

namespace dashboard.Sevices;
public class ItemService : IService<Item>
{
    private readonly ILogger<ItemService> _logger;
    private readonly DashboardDbContext _ctx;

    public ItemService(ILogger<ItemService> logger, DashboardDbContext ctx)
    {
        _logger = logger;
        _ctx = ctx;
    }
    public async Task<(bool IsSuccess, Exception Exception)> CreateAsync(Item newObject)
    {
        try
        {
            await _ctx.Items.AddAsync(newObject);
            await _ctx.SaveChangesAsync();

            return (true, null);
        }
        catch(Exception e)
        {
            return (false, e);
        }
    }

    public async Task<(bool IsSuccess, Exception Exception)> DeleteAsync(Guid id)
    {
        try
        {
            var item = await GetAsync(id);
            if(item == default)
            {
                _logger.LogInformation($"Item doesn't exist: {id}");
                return (false, new ArgumentException("Not found."));
            }
            _ctx.Items.Remove(item);
            await _ctx.SaveChangesAsync();
            _logger.LogInformation($"Item is deleted successfully: {item.Name} {id}");
            return (true, null);
        }
        catch(Exception e)
        {
            _logger.LogError($"Item isn't deleted, something wrong. Error message:\n {e.Message}");
            return (false, e);
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
        => await _ctx.Items.AnyAsync(i => i.ItemId == id);

    public async Task<List<Item>> GetAllAsync()
        => await _ctx.Items.ToListAsync();

    public async Task<Item> GetAsync(Guid id)
        => await _ctx.Items.FirstOrDefaultAsync(c => c.ItemId == id);

    public async Task<(bool IsSuccess, Exception Exception)> UpdateAsync(Item updatedObject)
    {
        try
        {
            _ctx.Items.Update(updatedObject);
            await _ctx.SaveChangesAsync();

            return (true, null);
        }
        catch(Exception e)
        {
            return (false, e);
        }
    }
}
