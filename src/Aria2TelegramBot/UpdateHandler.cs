using Aria2NET;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Aria2TelegramBot;

public class UpdateHandler(
    Aria2NetClient aria2,
    IHttpClientFactory httpClientFactory,
    IOptions<TelegramSettings> telegramSettings,
    IOptions<Aria2Settings> aria2Settings,
    IOptions<JellyfinSettings> jellyfinSettings,
    IOptions<ApiSettings> apiSettings) : IUpdateHandler
{

    private readonly string[] _declineMessages =
    [
        "I would love to say yes, but my dog told me to say no.",
        "I would, but I’m a teapot!",
        "418",
        "Request rejected!",
        "My parents would disown me if I did that.",
    ];

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
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

    private bool IsAuthorized(Message message)
    {
        if (message.From is not { } user)
        {
            return false;
        }

        return user.Id == telegramSettings.Value.AdminAccountId;
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
                else if (messageText.StartsWith("/dlstat"))
                {
                    await ReplyDownloadStatus(bot, chatId, message, parts);
                }
                else if (messageText.StartsWith("/vpn"))
                {
                    await ChangeVpnSettings(bot, chatId, message, parts);
                }

                break;
            }
        }
    }

    private async Task ChangeVpnSettings(ITelegramBotClient bot, long chatId, Message message, string[] parts)
    {
        var client = httpClientFactory.CreateClient();
        var url = $"http://{apiSettings.Value.Ip}:{apiSettings.Value.Ip}/vpn";
        bool connect;
        switch (parts[1])
        {
            case "connect":
                connect = true;
                break;
            case "disconnect":
                connect = false;
                break;
            default:
                await RequestDenied(bot, message);
                return;
        }

        var data = new
        {
            Connect = connect
        };
        
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync(url, content);
        
        if (response.IsSuccessStatusCode)
        {
            await bot.SendTextMessageAsync(chatId, connect ? "🔐" : "🔓", replyParameters: message, protectContent: true);
        }
        else
        {
            await bot.SendTextMessageAsync(chatId, "❌", replyParameters: message, protectContent: true);
        }
    }

    private async Task DownloadUrl(ITelegramBotClient bot, long chatId, Message message, string[] parts)
    {
        if (!IsAuthorized(message))
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

        var url = parts[1];
        var id = await aria2.AddUriAsync([url]);
        await bot.SendTextMessageAsync(chatId, $"download queued gid : `{id}`", replyParameters: message,
            parseMode: ParseMode.Markdown);
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

        var gid = parts[1];
        var status = await aria2.TellStatusAsync(gid);
        await bot.SendTextMessageAsync(chatId,
            $"{status.Gid} : {(status.CompletedLength / status.TotalLength) * 100:F2}", replyParameters: message);
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

    private async Task ServiceStatus(ITelegramBotClient bot, long chatId, Message message)
    {
        await bot.SendTextMessageAsync(chatId, $"""
                                                <pre>
                                                {GetRow("Service", "ℹ️")}
                                                {GetRow("Jellyfin", Ping(jellyfinSettings.Value.Ip, jellyfinSettings.Value.Port))}
                                                {GetRow("Aria2", Ping(aria2Settings.Value.Ip, aria2Settings.Value.Port))}
                                                </pre>
                                                """, parseMode: ParseMode.Html, replyParameters: message);
        return;

        static string GetRow(string key, string value)
        {
            return $"|{key,-15}|{value}|";
        }
    }

    private async Task WakeOnLan(ITelegramBotClient bot, long chatId, Message message)
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/wakeonlan",
                WorkingDirectory = "/usr/bin",
                Arguments = $"-i {apiSettings.Value.BroadcastIp} {apiSettings.Value.MacAddress}"
            }
        };

        process.Start();

        await bot.SendTextMessageAsync(chatId, "🌅🌞🥱", replyParameters: message);
    }

    private static string Ping(string ip, int port)
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