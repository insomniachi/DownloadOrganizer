using FluentValidation;
using Microsoft.Extensions.Options;

namespace Aria2TelegramBot;

public class FluentValidationOptions<TOptions> : IValidateOptions<TOptions>
    where TOptions : class
{
    private readonly IValidator<TOptions> _validator;
    public FluentValidationOptions(string name, IValidator<TOptions> validator)
    {
        _validator = validator;
    }
    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        var validationResult = _validator.Validate(options);

        if (validationResult.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Fail(validationResult.Errors.Select(x =>
            $"Validation for type {x.PropertyName} failed with error: {x.ErrorMessage}"));
    }
}