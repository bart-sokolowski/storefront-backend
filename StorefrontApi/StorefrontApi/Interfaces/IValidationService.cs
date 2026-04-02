using StorefrontApi.Validation;

namespace StorefrontApi.Interfaces;

public interface IValidationService
{
    Task<ValidationOutcome> ValidateAsync<TRequest>(TRequest request, CancellationToken ct = default);
}
