
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Server.Api;

public class ItemAdded
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

public class UserAuthorized
{
    public string NotificationUsername { get; set; } = string.Empty;
}

public class PlaybackStarted
{
    public string NotificationUsername { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PlaybackPosition { get; set; } = string.Empty;
    public string Runtime { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string SeasonNumber00 { get; set; } = string.Empty;
    public string EpisodeNumber00 { get; set; } = string.Empty;
}

public class PlaybackStopped
{
    public string NotificationUsername { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PlaybackPosition { get; set; } = string.Empty;
    public string Runtime { get; set; } = string.Empty;
    public bool PlayedToCompletion { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public string SeasonNumber00 { get; set; } = string.Empty;
    public string EpisodeNumber00 { get; set; } = string.Empty;
}

public class PluginInfo
{
    public string PluginName { get; set; } = string.Empty;
    public string PluginVersion { get; set; } = string.Empty;
    public string PluginChangelog { get; set; } = string.Empty;
}