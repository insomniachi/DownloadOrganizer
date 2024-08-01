using System.Text.Json.Serialization;

namespace DownloadOrganizer;

public class AppSettings
{
	public string DownloadsDirectory { get; set; } = "Downloads";
	public string MediaDirectory { get; set; } = "Media";
}

[JsonSerializable(typeof(AppSettings))]
public partial class AppSettingsJsonSerializerContext : JsonSerializerContext
{

	
}