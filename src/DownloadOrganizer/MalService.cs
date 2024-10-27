using System.Text.Json;
using System.Text.Json.Serialization;

namespace DownloadOrganizer;

public static class MalService
{
	private static readonly HttpClient HttpClient = new() { BaseAddress = new Uri("https://api.myanimelist.net/v2/") };

	static MalService()
	{
		HttpClient.DefaultRequestHeaders.Add("X-MAL-CLIENT-ID", "748da32a6defdd448c1f47d60b4bbe69");
	}

	public static async Task<bool> IsAnime(string name)
	{
		try
		{
			var stream = await HttpClient.GetStreamAsync($"anime?q={name}&limit=1&fields=alternative_titles");
			var response = await JsonSerializer.DeserializeAsync(stream, AppJsonSerializerContext.Default.Response);

			if (response.Data is not { Length: 1 })
			{
				return false;
			}

			var anime = response.Data[0].Node;

			return string.Equals(name, anime.Title, StringComparison.OrdinalIgnoreCase) ||
				   string.Equals(name, anime.AlternativeTitles.English, StringComparison.OrdinalIgnoreCase) ||
				   string.Equals(name, anime.AlternativeTitles.Japanese, StringComparison.OrdinalIgnoreCase) ||
				   anime.AlternativeTitles.Synonyms.Any(x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase));
		}
		catch
		{
			return false;
		}
	}
}

public class Response
{
	[JsonPropertyName("data")]
	public AnimeNode[] Data { get; set; }
}

public class AnimeNode
{
	[JsonPropertyName("node")]
	public Anime Node { get; set; }
}

public class Anime
{
	[JsonPropertyName("title")]
	public string Title { get; set; }

	[JsonPropertyName("alternative_titles")]
	public AlternativeTitles AlternativeTitles { get; set; }
}

public class AlternativeTitles
{
	[JsonPropertyName("synonyms")]
	public string[] Synonyms { get; set; }

	[JsonPropertyName("en")]
	public string English { get; set; }

	[JsonPropertyName("ja")]
	public string Japanese { get; set; }
}
