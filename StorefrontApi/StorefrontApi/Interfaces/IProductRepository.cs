namespace StorefrontApi.Interfaces;

using StorefrontApi.Models;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllActiveAsync(CancellationToken ct = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Product> AddAsync(Product product, CancellationToken ct = default);
    Task<bool> ArchiveAsync(Guid id, CancellationToken ct = default);
    Task<bool> DecrementStockAsync(Guid id, int quantity, CancellationToken ct = default);
    Task<bool> IncrementStockAsync(Guid id, int quantity, CancellationToken ct = default);
}
