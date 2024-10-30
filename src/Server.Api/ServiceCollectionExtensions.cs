using Aria2TelegramBot;
using FluentValidation;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Server.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegramBot(this IServiceCollection services)
    {
        return services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var telegramSettings = sp.GetRequiredService<IOptions<TelegramSettings>>();
            return new TelegramBotClient(telegramSettings.Value.BotToken);
        });
    }
    
    public static OptionsBuilder<TOptions> ValidateFluently<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder)
        where TOptions : class
    {
        optionsBuilder.Services.AddTransient<IValidateOptions<TOptions>>(x => new FluentValidationOptions<TOptions>(optionsBuilder.Name, x.GetRequiredService<IValidator<TOptions>>()));
        return optionsBuilder;
    }
}