using ConsoleAppFramework;

namespace DownloadOrganizer;

public static class Commands
{
	/// <summary>
	/// Move a single directory/archive to media folder
	/// </summary>
	/// <param name="directoryToMove">full path to directory/archive</param>
	public static void MoveDirectory([Argument] string directoryToMove)
	{
		if (string.IsNullOrEmpty(directoryToMove))
		{
			return;
		}

		Organizer.Organize(directoryToMove);
	}

	/// <summary>
	/// Organize the contents of your downloads directory into the media directory in a single go
	/// </summary>
	/// <param name="downloadsDirectory">full path to downloads directory</param>
	/// <param name="mediaDirectory">base path of media directory, subdirectories will be created by the tool</param>
	public static void OrganizeDownloadsDirectory([Argument] string downloadsDirectory, [Argument] string mediaDirectory)
	{
		if (string.IsNullOrEmpty(downloadsDirectory) || string.IsNullOrEmpty(mediaDirectory))
		{
			return;
		}

		Organizer.Organize(downloadsDirectory, mediaDirectory);
	}

	/// <summary>
	///  Organize the contents of your downloads directory into the media directory in a single go configured in appsettings
	/// </summary>
	public static void Run() => Organizer.Organize();
}