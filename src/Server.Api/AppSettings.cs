namespace Server.Api;

public class TelegramSettings
{
    public required string BotToken { get; init; }
    public required long GroupChatId { get; init; }
}

public class JellyfinSettings
{
    public required string WebUrl { get; init; }
    public required string ApiToken { get; init; }
    public required string LocalUrl { get; init; }
}