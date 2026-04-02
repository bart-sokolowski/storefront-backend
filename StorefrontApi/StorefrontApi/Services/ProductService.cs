namespace StorefrontApi.Services;

using StorefrontApi.Common;
using StorefrontApi.DTOs.Requests;
using StorefrontApi.DTOs.Responses;
using StorefrontApi.Interfaces;
using StorefrontApi.Models;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IValidationService _validationService;

    public ProductService(IProductRepository repository, IValidationService validationService)
    {
        _repository = repository;
        _validationService = validationService;
    }

    public async Task<ApiResult<IEnumerable<ProductResponse>>> GetAllAsync(CancellationToken ct = default)
    {
        var products = await _repository.GetAllActiveAsync(ct);
        return ApiResult<IEnumerable<ProductResponse>>.Ok(products.Select(MapToResponse));
    }

    public async Task<ApiResult<ProductResponse>> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        var validation = await _validationService.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ApiResult<ProductResponse>.Fail("Validation failed.", validation.Errors);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(product, ct);
        return ApiResult<ProductResponse>.Ok(MapToResponse(product), "Product created.");
    }

    public async Task<ApiResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        if (product is null)
            return ApiResult.Fail("Product not found.");

        if (product.Status == ProductStatus.Archived)
            return ApiResult.Ok("Product is already archived.");

        await _repository.ArchiveAsync(id, ct);
        return ApiResult.Ok("Product archived successfully.");
    }

    private static ProductResponse MapToResponse(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Stock = product.Stock,
        CreatedAt = product.CreatedAt
    };
}
