using FluentValidation;
// ReSharper disable UnusedType.Global

namespace Aria2TelegramBot;

public class Aria2SettingsValidator : AbstractValidator<Aria2Settings>
{
    public Aria2SettingsValidator()
    {
        RuleFor(x => x.RpcUrl)
            .NotNull().NotEmpty();

        RuleFor(x => x.Secret)
            .NotNull().NotEmpty();
    }
}

public class TelegramSettingsValidator : AbstractValidator<TelegramSettings>
{
    public TelegramSettingsValidator()
    {
        RuleFor(x => x.BotToken)
            .NotNull().NotEmpty();
    }
}