using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace Aria2TelegramBot;

public class Worker(ITelegramBotClient bot, UpdateHandler handler) : BackgroundService
{
	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		bot.DropPendingUpdatesAsync(stoppingToken);
		bot.StartReceiving(handler, cancellationToken: stoppingToken);
		return Task.CompletedTask;
	}
}