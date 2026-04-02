namespace StorefrontApi.Validation;

using FluentValidation;
using StorefrontApi.Interfaces;

public class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<ValidationOutcome> ValidateAsync<TRequest>(TRequest request, CancellationToken ct = default)
    {
        var validator = _serviceProvider.GetService<IValidator<TRequest>>();
        if (validator is null)
            return ValidationOutcome.Success();

        var result = await validator.ValidateAsync(request, ct);
        if (result.IsValid)
            return ValidationOutcome.Success();

        return ValidationOutcome.Failure(result.Errors.Select(e => e.ErrorMessage));
    }
}
