using System.Text;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Server.Api;

// ReSharper disable once ClassNeverInstantiated.Global
public class JellyfinWebhook(ITelegramBotClient bot,
                             IHttpClientFactory httpClientFactory,
                             IOptions<JellyfinSettings> jellyfinSettings,
                             IOptions<TelegramSettings> telegramSettings)
{
    private readonly JellyfinSettings _jellyfinSettings = jellyfinSettings.Value;
    private readonly TelegramSettings _telegramSettings = telegramSettings.Value;
    
    public async Task HandleItemAdded(ItemAdded? item)
    {
        if (item is null)
        {
            return;
        }
		
        var webUrl = _jellyfinSettings.WebUrl;
        var localUrl = _jellyfinSettings.LocalUrl;
        var jellyfinToken = _jellyfinSettings.ApiToken;
		
        var httpClient = httpClientFactory.CreateClient();
        var photoStream = await httpClient.GetStreamAsync($"{localUrl}/Items/{item.ItemId}/Images/Primary");
        var memoryStream = new MemoryStream();
        await photoStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
		
        var caption = item.ItemType switch
        {
            "Season" => $"<b>{item.ItemType} Now Available</b>\n\n" + 
                        $"<b>{item.SeriesName} ({item.Year})</b>\n" +
                        $"<b><em>{item.Name}</em></b>\n" +
                        $"{item.Overview}\n\n" +
                        $"<b>Runtime</b> : {item.Runtime}",
            "Episode" => $"<b>{item.ItemType} Now Available</b>\n\n" +
                         $"<b>{item.SeriesName} ({item.Year})</b> - <b>S{item.SeasonNumber00}E{item.EpisodeNumber00}</b> - <b><em>{item.Name}</em></b>\n" +
                         $"{item.Overview}\n\n" +
                         $"<b>Runtime</b> : {item.Runtime}\n\n" +
                         $"<a href=\"{webUrl}/Items/{item.ItemId}/Download?api_key={jellyfinToken}\">Download Now!</a>",
            _ => $"<b>{item.ItemType} Now Available</b>\n\n" + 
                 $"<b>{item.Name} ({item.Year})</b>\n" +
                 $"{item.Overview}\n\n" +
                 $"<b>Runtime</b> : {item.Runtime}\n\n" +
                 $"<a href=\"{webUrl}/Items/{item.ItemId}/Download?api_key={jellyfinToken}\">Download Now!</a>"
        };

        await bot.SendPhotoAsync(_telegramSettings.GroupChatId, InputFile.FromStream(memoryStream), caption: caption, parseMode: ParseMode.Html, protectContent: true);
    }

    public async Task HandleAuthenticationSuccess(UserAuthorized? item)
    {
        if (item is null)
        {
            return;
        }

        if (IsAdminUser(item.NotificationUsername))
        {
            return;
        }
        
        await bot.SendTextMessageAsync(_telegramSettings.GroupChatId, $"User {item.NotificationUsername} logged in", protectContent: true);
    }

    public async Task HandlerPlaybackStarted(PlaybackStarted? item)
    {
        if (item is null)
        {
            return;
        }

        if (IsAdminUser(item.NotificationUsername))
        {
            return;
        }

        await bot.SendTextMessageAsync(_telegramSettings.GroupChatId, GetMessage(item), parseMode: ParseMode.Html, protectContent: true);
        return;
        
        string GetMessage(PlaybackStarted playbackItem)
        {
            var sb = new StringBuilder();
            sb.Append($"{playbackItem.NotificationUsername} started playing {playbackItem.Name}");
            if (playbackItem.ItemType == "Episode")
            {
                sb.Append($" S{playbackItem.SeasonNumber00}E{playbackItem.EpisodeNumber00} ");
            }

            sb.Append($"from ({playbackItem.PlaybackPosition}/{playbackItem.Runtime}) on {item.DeviceName} ({item.ClientName})");

            return sb.ToString();
        }
    }
    
    public async Task HandlerPlaybackStopped(PlaybackStopped? item)
    {
        if (item is null)
        {
            return;
        }

        if (IsAdminUser(item.NotificationUsername))
        {
            return;
        }
        
        await bot.SendTextMessageAsync(_telegramSettings.GroupChatId, GetMessage(item), parseMode: ParseMode.Html, protectContent: true);
        return;

        string GetMessage(PlaybackStopped playbackItem)
        {
            var sb = new StringBuilder();
            if (playbackItem.PlayedToCompletion)
            {
                sb.Append($"{playbackItem.NotificationUsername} completed {playbackItem.Name}");
                if (playbackItem.ItemType == "Episode")
                {
                    sb.Append($" S{playbackItem.SeasonNumber00}E{playbackItem.EpisodeNumber00}");
                }

                return sb.ToString();
            }
            
            sb.Append($"{playbackItem.NotificationUsername} stopped playing {playbackItem.Name}");
            if (playbackItem.ItemType == "Episode")
            {
                sb.Append($" S{playbackItem.SeasonNumber00}E{playbackItem.EpisodeNumber00} ");
            }

            sb.Append($"at ({playbackItem.PlaybackPosition}/{playbackItem.Runtime})");

            return sb.ToString();
        }

    }

    public async Task HandlePluginUpdated(PluginInfo? item)
    {
        if (item is null)
        {
            return;
        }
        
        await bot.SendTextMessageAsync(_telegramSettings.GroupChatId, $"<b>{item.PluginName}</b> updated to <b>{item.PluginVersion}</b>\n<b>Changelog</b>\n{item.PluginChangelog}", parseMode: ParseMode.Html, protectContent: true);
    }
    
    public async Task HandlePluginInstalled(PluginInfo? item)
    {
        if (item is null)
        {
            return;
        }
        
        await bot.SendTextMessageAsync(_telegramSettings.GroupChatId, $"<b>{item.PluginName} ({item.PluginVersion}) installed", parseMode: ParseMode.Html, protectContent: true);
    }

    private bool IsAdminUser(string username) => _jellyfinSettings.AdminUser == username;
}