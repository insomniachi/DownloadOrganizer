using FluentValidation;

// ReSharper disable UnusedType.Global

namespace Server.Api;


public class TelegramSettingsValidator : AbstractValidator<TelegramSettings>
{
    public TelegramSettingsValidator()
    {
        RuleFor(x => x.BotToken)
            .NotNull().NotEmpty();

        RuleFor(x => x.GroupChatId)
            .NotEmpty();
    }
}

public class JellyfinSettingsValidator : AbstractValidator<JellyfinSettings>
{
    public JellyfinSettingsValidator()
    {
        RuleFor(x => x.LocalUrl)
            .NotNull().NotEmpty();

        RuleFor(x => x.WebUrl)
            .NotNull().NotEmpty();

        RuleFor(x => x.ApiToken)
            .NotNull().NotEmpty();

        RuleFor(x => x.AdminUser)
            .NotNull().NotEmpty();
    }
}