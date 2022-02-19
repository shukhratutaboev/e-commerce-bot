namespace dashboard.Sevices;
public interface IService<T>
{
    Task<(bool IsSuccess, Exception Exception)> CreateAsync(T newObject);
    Task<List<T>> GetAllAsync();
    Task<T> GetAsync(Guid id);
    Task<(bool IsSuccess, Exception Exception)> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}