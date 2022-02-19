using dashboard.Entities;
using Microsoft.EntityFrameworkCore;

namespace dashboard.Sevices;
public class CategoryService : IService<Category>
{
    private readonly ILogger<CategoryService> _logger;
    private readonly DashboardDbContext _ctx;

    public CategoryService(ILogger<CategoryService> logger, DashboardDbContext ctx)
    {
        _logger = logger;
        _ctx = ctx;
    }
    public async Task<(bool IsSuccess, Exception Exception)> CreateAsync(Category newObject)
    {
        try
        {
            await _ctx.Categories.AddAsync(newObject);
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
            var category = await GetAsync(id);
            if(category == default)
            {
                _logger.LogInformation($"Category doesn't exist: {id}");
                return (false, new ArgumentException("Not found."));
            }
            _ctx.Categories.Remove(category);
            await _ctx.SaveChangesAsync();
            _logger.LogInformation($"Category deleted successfully: {category.Name} {id}");
            return (true, null);
        }
        catch(Exception e)
        {
            _logger.LogError($"Category can't be deleted. Error message:\n {e.Message}");
            return (false, e);
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
        => await _ctx.Categories.AnyAsync(c => c.Id == id);

    public async Task<List<Category>> GetAllAsync()
        => await _ctx.Categories.ToListAsync();

    public async Task<Category> GetAsync(Guid id)
        => await _ctx.Categories.FirstOrDefaultAsync(c => c.Id == id);
}