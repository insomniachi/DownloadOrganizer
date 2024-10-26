using Aria2NET;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Aria2TelegramBot;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAria2(this IServiceCollection services)
    {
        return services.AddSingleton(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var jsonRpcUrl = configuration.GetValue<string>("Aria2Url");
            var secret = configuration.GetValue<string>("Aria2Secret");

            if (string.IsNullOrEmpty(jsonRpcUrl) || string.IsNullOrEmpty(secret))
            {
                throw new Exception("Please provide Aria2Url and Aria2Secret");
            }
    
            return new Aria2NetClient(jsonRpcUrl, secret); 
        });
    }

    public static IServiceCollection AddTelegramBot(this IServiceCollection services)
    {
        return services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var botToken = configuration.GetValue<string>("TelegramBotToken");

            if (string.IsNullOrEmpty(botToken))
            {
                throw new Exception("Please provide TelegramBotToken");
            }

            return new TelegramBotClient(botToken);
        });
    }
}