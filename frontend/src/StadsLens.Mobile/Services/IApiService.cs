namespace StadsLens.Mobile.Services;

// Generiek contract met alle CRUD-methodes voor één resource van de API.
public interface IApiService<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T?> CreateAsync(T item);
    Task<bool> UpdateAsync(int id, T item);
    Task<bool> DeleteAsync(int id);
}
