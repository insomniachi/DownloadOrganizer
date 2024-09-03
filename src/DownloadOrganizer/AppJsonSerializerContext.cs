using System.Text.Json.Serialization;

namespace DownloadOrganizer;

[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(Response))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{

	
}