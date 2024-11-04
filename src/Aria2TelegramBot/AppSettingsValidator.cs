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

        RuleFor(x => x.Ip)
            .NotNull().NotEmpty();

        RuleFor(x => x.Port)
            .NotEmpty();
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

public class JellyfinSettingsValidator : AbstractValidator<JellyfinSettings>
{
    public JellyfinSettingsValidator()
    {
        RuleFor(x => x.Ip)
            .NotNull().NotEmpty();

        RuleFor(x => x.Port)
            .NotEmpty();
    }
}

public class ApiSettingsValidator : AbstractValidator<ApiSettings>
{
    public ApiSettingsValidator()
    {
        RuleFor(x => x.Ip)
            .NotNull().NotEmpty();
        
        RuleFor(x => x.BroadcastIp)
            .NotNull().NotEmpty();
        
        RuleFor(x => x.MacAddress)
            .NotNull().NotEmpty();

        RuleFor(x => x.Port)
            .NotEmpty();
    }
}