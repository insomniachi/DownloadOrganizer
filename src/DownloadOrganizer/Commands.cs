using ConsoleAppFramework;

namespace DownloadOrganizer;

public static class Commands
{
	/// <summary>
	/// Move a single directory/archive to media folder
	/// </summary>
	/// <param name="directoryToMove">full path to directory/archive</param>
	public static async Task MoveDirectory([Argument] string directoryToMove)
	{
		if (string.IsNullOrEmpty(directoryToMove))
		{
			return;
		}

		await Organizer.Organize(directoryToMove);
	}

	/// <summary>
	/// Organize the contents of your downloads directory into the media directory in a single go
	/// </summary>
	/// <param name="downloadsDirectory">full path to downloads directory</param>
	/// <param name="mediaDirectory">base path of media directory, subdirectories will be created by the tool</param>
	public static async Task OrganizeDownloadsDirectory([Argument] string downloadsDirectory, [Argument] string mediaDirectory)
	{
		if (string.IsNullOrEmpty(downloadsDirectory) || string.IsNullOrEmpty(mediaDirectory))
		{
			return;
		}

		await Organizer.Organize(downloadsDirectory, mediaDirectory);
	}

	/// <summary>
	///  Organize the contents of your downloads directory into the media directory in a single go configured in app settings
	/// </summary>
	public static Task Run() => Organizer.Organize();
}