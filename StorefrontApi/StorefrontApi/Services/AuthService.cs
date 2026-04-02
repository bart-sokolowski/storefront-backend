namespace StorefrontApi.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StorefrontApi.Common;
using StorefrontApi.DTOs.Requests;
using StorefrontApi.DTOs.Responses;
using StorefrontApi.Interfaces;

public class AuthService : IAuthService
{
    private static readonly Dictionary<string, (string Password, string Role)> Users = new()
    {
        { "user@storefront.com", ("password123", "User") },
        { "admin@storefront.com", ("password123", "Admin") }
    };

    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<ApiResult<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (!Users.TryGetValue(request.Email.ToLower(), out var user) || user.Password != request.Password)
            return Task.FromResult(ApiResult<AuthResponse>.Fail("Invalid credentials."));

        var token = GenerateToken(request.Email, user.Role);
        return Task.FromResult(ApiResult<AuthResponse>.Ok(new AuthResponse { Token = token }));
    }

    private string GenerateToken(string email, string role)
    {
        var secret = _configuration["Jwt:Secret"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
