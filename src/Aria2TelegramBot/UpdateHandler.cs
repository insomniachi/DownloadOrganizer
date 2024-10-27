using Aria2NET;
using System.Diagnostics;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Aria2TelegramBot;

public class UpdateHandler(Aria2NetClient aria2, 
	                       IHttpClientFactory httpClientFactory,
	                       IConfiguration configuration) : IUpdateHandler
{
	private readonly long _admin = configuration.GetValue<long>("TelegramAdmin");
	
	private readonly string[] _declineMessages =
	[
		"I would love to say yes, but my dog told me to say no.",
		"I would, but I’m a teapot!",
		"418",
		"Request rejected!",
		"My parents would disown me if I did that.",
	];

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

	private async Task<bool> IsAuthorized(ITelegramBotClient bot, Message message)
	{
		var chatId = message.Chat.Id;
		
		if (message.From is not { } user)
		{
			return false;
		}
		
		var userId = user.Id;

		if (userId == _admin)
		{
			return true;
		}

		var administrators = await bot.GetChatAdministratorsAsync(chatId);

		return administrators.Any(x => x.User.Id == userId) &&
		       administrators.FirstOrDefault(x => x.User.Id == _admin) is not null;
	}

	private async Task OnMessage(ITelegramBotClient bot, Message message)
	{
		if (message.Text is not { } messageText)
		{
			return;
		}
		
		var chatId = message.Chat.Id;
		var parts = messageText.Split(' ');
		
		switch (messageText)
		{
			case "/aria2":
				await ReplyAria2Version(bot, chatId, message);
				break;
			case "/status":
				await ServiceStatus(bot, chatId, message);
				break;
			case "/wol":
				await WakeOnLan(bot, chatId, message);
				break;
			case "/ip":
				await ReplyIp(bot, chatId, message);
				break;
			default:
			{
				if (messageText.StartsWith("/download"))
				{
					await DownloadUrl(bot, chatId, message, parts);
				}
				else if(messageText.StartsWith("/dlstat"))
				{
					await ReplyDownloadStatus(bot, chatId, message, parts);
				}
				break;
			}
		}
	}

	private async Task DownloadUrl(ITelegramBotClient bot, long chatId, Message message, string[] parts)
	{
		if (!await IsAuthorized(bot, message))
		{
			await RequestDenied(bot, message);
			return;
		}
					
		if (parts.Length < 2)
		{
			const string reply = "/download ❌\n/download <url> ✅";
			await bot.SendTextMessageAsync(chatId, reply, replyParameters: message);
			return;
		}

		var url = parts[2];
		var id = await aria2.AddUriAsync([url]);
		await bot.SendTextMessageAsync(chatId, $"download queued gid : `{id}`", replyParameters: message, parseMode: ParseMode.Markdown);
	}

	private async Task ReplyAria2Version(ITelegramBotClient bot, long chatId, Message message)
	{
		var version = await aria2.GetVersionAsync();
		await bot.SendTextMessageAsync(chatId, version.Version, replyParameters: message);
	}

	private async Task ReplyDownloadStatus(ITelegramBotClient bot, long chatId, Message message, string[] parts)
	{
		if (parts.Length < 2)
		{
			const string reply = "/dlstat ❌\n/dlstat <gid> ✅";
			await bot.SendTextMessageAsync(chatId, reply, replyParameters: message);
			return;
		}
		
		var gid = parts[2];
		var status = await aria2.TellStatusAsync(gid);
		await bot.SendTextMessageAsync(chatId, $"{status.Gid} : {(status.CompletedLength/status.TotalLength) * 100 :F2}", replyParameters: message);
	}

	private async Task ReplyIp(ITelegramBotClient bot, long chatId, Message message)
	{
		var client = httpClientFactory.CreateClient();
		var ip = await client.GetStringAsync("https://icanhazip.com/");
		await bot.SendTextMessageAsync(chatId, ip, replyParameters: message);
	}

	private async Task RequestDenied(ITelegramBotClient bot, Message message)
	{
		var randomIndex = Random.Shared.Next(0, _declineMessages.Length - 1);
		var declineMessage = _declineMessages[randomIndex];
		await bot.SendTextMessageAsync(message.Chat.Id, declineMessage, replyParameters: message);
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
				""", parseMode: ParseMode.Html, replyParameters: message);
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