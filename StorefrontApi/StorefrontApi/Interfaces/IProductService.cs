namespace StorefrontApi.Interfaces;

using StorefrontApi.Common;
using StorefrontApi.DTOs.Requests;
using StorefrontApi.DTOs.Responses;

public interface IProductService
{
    Task<ApiResult<IEnumerable<ProductResponse>>> GetAllAsync(CancellationToken ct = default);
    Task<ApiResult<ProductResponse>> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<ApiResult> DeleteAsync(Guid id, CancellationToken ct = default);
}
