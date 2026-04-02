namespace StorefrontApi.Interfaces;

using StorefrontApi.Common;
using StorefrontApi.DTOs.Requests;
using StorefrontApi.DTOs.Responses;

public interface IAuthService
{
    Task<ApiResult<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
