namespace Aria2TelegramBot;

public class Aria2Settings
{
    public required string RpcUrl { get; init; }
    public required string Secret { get; init; }
}

public class TelegramSettings
{
    public required long AdminAccountId { get; init; }
    public required string BotToken { get; init; }
}