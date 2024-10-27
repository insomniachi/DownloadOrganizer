using Aria2NET;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Aria2TelegramBot;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAria2(this IServiceCollection services)
    {
        return services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<Aria2Settings>>().Value;
            return new Aria2NetClient(settings.RpcUrl, settings.Secret); 
        });
    }

    public static IServiceCollection AddTelegramBot(this IServiceCollection services)
    {
        return services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<TelegramSettings>>().Value;
            return new TelegramBotClient(settings.BotToken);
        });
    }
    
    public static OptionsBuilder<TOptions> ValidateFluently<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder)
        where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(x => new FluentValidationOptions<TOptions>(optionsBuilder.Name, x.GetRequiredService<IValidator<TOptions>>()));
        return optionsBuilder;
    }
}