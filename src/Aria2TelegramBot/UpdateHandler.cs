using Aria2NET;
using System.Diagnostics;
using System.Net.Sockets;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Aria2TelegramBot;

public class UpdateHandler(Aria2NetClient aria2) : IUpdateHandler
{
	// private readonly long[] _admins = [147093641];
	// private readonly string[] _declineMessages =
	// [
	// 	"I would love to say yes, but my dog told me to say no.",
	// 	"I would, but I’m a teapot!",
	// 	"418",
	// 	"Request rejected!",
	// 	"My parents would disown me if I did that.",
	//
	// ];

	public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		await (update switch
		{
			{ Message: { } message } => OnMessage(bot, message),
			_ => Task.CompletedTask,
		});
	}

	// private bool IsAuthorized(long id)
	// {
	// 	return _admins.Contains(id);
	// }

	private async Task OnMessage(ITelegramBotClient bot, Message message)
	{
		if (message.Text is not { } messageText)
		{
			return;
		}

		var chatId = message.Chat.Id;
		var parts = messageText.Split(' ');
		
		if (messageText is "/aria2")
		{
			await GetAria2Version(bot, chatId, message);
		}
		else if (messageText.StartsWith("/download"))
		{
			if (parts.Length < 2)
			{
				await bot.SendTextMessageAsync(chatId, "/download <url>", replyParameters: message);
			}

			await Download(bot, chatId, message, parts[2]);
		}
		else if(messageText.StartsWith("/dlstat"))
		{
			if (parts.Length < 2)
			{
				await bot.SendTextMessageAsync(chatId, "/dlstat <gid>", replyParameters: message);
			}

			await DownloadStatus(bot, chatId, message, parts[2]);
		}
		else if(messageText is "/status")
		{
			await ServiceStatus(bot, chatId, message);
		}
		else if(messageText is "/wol")
		{
			await WakeOnLan(bot, chatId, message);
		}
	}

	private async Task Download(ITelegramBotClient bot, long chatId, Message messageId, string url)
	{
		var id = await aria2.AddUriAsync([url]);
		await bot.SendTextMessageAsync(chatId, $"download queued gid : `{id}`", replyParameters: messageId, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
	}

	private async Task GetAria2Version(ITelegramBotClient bot, long chatId, Message messageId)
	{
		var version = await aria2.GetVersionAsync();
		await bot.SendTextMessageAsync(chatId, version.Version, replyParameters: messageId);
	}

	private async Task DownloadStatus(ITelegramBotClient bot, long chatId, Message messageId, string gid)
	{
		var status = await aria2.TellStatusAsync(gid);
		await bot.SendTextMessageAsync(chatId, $"{status.Gid} : {(status.CompletedLength/status.TotalLength) * 100 :F2}", replyParameters: messageId);
	}

	private static async Task ServiceStatus(ITelegramBotClient bot, long chatId, Message message)
	{
		const string ip = "192.168.1.200";
		await bot.SendTextMessageAsync(chatId, $"""
				<pre>
				{GetRow("Service", "ℹ️")}
				{GetRow("Jellyfin", Ping(ip, 8096))}
				{GetRow("Aria2", Ping(ip, 6800))}
				</pre>
				""", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyParameters: message);
		return;

		static string GetRow(string key, string value)
		{
			return $"|{key,-15}|{value}|";
		}
	}

	private static async Task WakeOnLan(ITelegramBotClient bot, long chatId, Message message)
	{
		var process = new Process()
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "/usr/bin/wakeonlan",
				WorkingDirectory = "/usr/bin",
				Arguments = "-i 192.168.1.255 74:56:3C:13:85:83"
			}
		};

		process.Start();

		await bot.SendTextMessageAsync(chatId, "🌅🌞🥱", replyParameters: message);
	}

	private static string Ping(string ip,  int port)
	{
		try
		{
			using var client = new TcpClient(ip, port);
			return "✅";
		}
		catch
		{
			return "❌";
		}
	}
}