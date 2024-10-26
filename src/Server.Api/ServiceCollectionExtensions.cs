using Telegram.Bot;

namespace Server.Api;

public static class ServiceCollectionExtensions
{
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