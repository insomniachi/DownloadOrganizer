namespace Aria2TelegramBot;

public class Aria2Settings
{
    public required string RpcUrl { get; init; }
    public required string Secret { get; init; }
    public required string Ip { get; init; }
    public required int Port { get; init; }
}

public class TelegramSettings
{
    public required long AdminAccountId { get; init; }
    public long[] DownloaderAccountIds { get; set; } = [];
    public required string BotToken { get; init; }
}

public class JellyfinSettings
{
    public required string Ip { get; init; }
    public required int Port { get; init; }
}

public class ApiSettings
{
    public required string Ip { get; init; }
    public required int Port { get; init; }
    public required string BroadcastIp { get; init; }
    public required string MacAddress { get; init; }
}