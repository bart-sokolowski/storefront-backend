namespace StorefrontApi.Validation;

public class ValidationOutcome
{
    public bool IsValid { get; set; }
    public IEnumerable<string> Errors { get; set; } = [];

    public static ValidationOutcome Success() => new() { IsValid = true };
    public static ValidationOutcome Failure(IEnumerable<string> errors) => new() { IsValid = false, Errors = errors };
}
