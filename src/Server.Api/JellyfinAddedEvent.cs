using System.Diagnostics.CodeAnalysis;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Server.Api;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class JellyfinAddedEvent
{
	public string ItemType { get; set; } = string.Empty;
	public string SeriesName { get; set; } = string.Empty;
	public int? Year { get; set; }
	public string SeasonNumber00 { get; set; } = string.Empty;
	public string EpisodeNumber00 { get; set; } = string.Empty;
	public string Overview { get; set; } = string.Empty;
	public string Runtime { get; set; } = string.Empty;
	public string ItemId { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
}

public static class JellyfinWebhook
{
	public static async Task HandleItemAdded(JellyfinAddedEvent item, ITelegramBotClient bot, IHttpClientFactory httpClientFactory, IConfiguration config)
	{
		var jellyfinToken = config.GetValue<string>("JellyfinToken");
		var baseUrl = config.GetValue<string>("JellyfinBaseUrl");
		
		var httpClient = httpClientFactory.CreateClient();
		var photoStream = await httpClient.GetStreamAsync($"http://192.168.1.200:8096/jellyfin/Items/{item.ItemId}/Images/Primary");
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
			             $"<a href=\"{baseUrl}/Items/{item.ItemId}/Download?api_key={jellyfinToken}\">Download Now!</a>",
			_ => $"<b>{item.ItemType} Now Available</b>\n\n" + 
			     $"<b>{item.Name} ({item.Year})</b>\n" +
			     $"{item.Overview}\n\n" +
			     $"<b>Runtime</b> : {item.Runtime}\n\n" +
			     $"<a href=\"{baseUrl}/Items/{item.ItemId}/Download?api_key={jellyfinToken}\">Download Now!</a>"
		};

		await bot.SendPhotoAsync(-1002462609458, InputFile.FromStream(memoryStream), caption: caption, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, protectContent: true);
	}
}