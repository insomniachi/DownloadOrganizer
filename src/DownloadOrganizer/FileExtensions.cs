namespace DownloadOrganizer;

internal static class FileExtensions
{
	public static string[] IgnoredFiles { get; } = [".txt", ".nfo", ".jpg"];
	public static string[] MediaFiles { get; } = [".mkv", ".mp4"];
	public static string Zip { get; } = ".zip";
}
