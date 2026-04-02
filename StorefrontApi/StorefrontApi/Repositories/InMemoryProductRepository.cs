namespace StorefrontApi.Repositories;

using System.Collections.Concurrent;
using StorefrontApi.Interfaces;
using StorefrontApi.Models;

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _products;

    public InMemoryProductRepository(IEnumerable<Product> seed)
    {
        _products = new ConcurrentDictionary<Guid, Product>(seed.ToDictionary(p => p.Id));
    }

    public Task<IEnumerable<Product>> GetAllActiveAsync(CancellationToken ct = default)
    {
        var active = _products.Values.Where(p => p.Status == ProductStatus.Active);
        return Task.FromResult(active);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _products.TryGetValue(id, out var product);
        return Task.FromResult(product);
    }

    public Task<Product> AddAsync(Product product, CancellationToken ct = default)
    {
        _products[product.Id] = product;
        return Task.FromResult(product);
    }

    public Task<bool> ArchiveAsync(Guid id, CancellationToken ct = default)
    {
        if (!_products.TryGetValue(id, out var product))
            return Task.FromResult(false);

        product.Status = ProductStatus.Archived;
        return Task.FromResult(true);
    }

    public Task<bool> DecrementStockAsync(Guid id, int quantity, CancellationToken ct = default)
    {
        if (!_products.TryGetValue(id, out var product) || product.Stock < quantity)
            return Task.FromResult(false);

        product.Stock -= quantity;
        return Task.FromResult(true);
    }

    public Task<bool> IncrementStockAsync(Guid id, int quantity, CancellationToken ct = default)
    {
        if (!_products.TryGetValue(id, out var product))
            return Task.FromResult(false);

        product.Stock += quantity;
        return Task.FromResult(true);
    }
}
