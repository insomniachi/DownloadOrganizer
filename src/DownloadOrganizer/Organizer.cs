using DownloadOrganizer.Archiving;

namespace DownloadOrganizer;

public static class Organizer
{
	private static readonly string[] _mediaExtentions =
	[
		".mkv",
		".mp4"
	];

	public static void Organize(string sourceDir, string targetDir)
	{
		var fileSystem = new SystemIoFileSystem();
		var mover = new MediaMover(fileSystem);
		var archiveExtractor = new ArchiveExtractor(fileSystem, [new ZipArchiver()]);

		archiveExtractor.ExtractAllArchives(sourceDir);

		foreach (var mediaFolder in fileSystem.GetDirectories(sourceDir))
		{
			mover.MoveFolder(mediaFolder, targetDir);
		}
		foreach (var mediaFile in fileSystem.GetFiles(sourceDir, "*.*", SearchOption.TopDirectoryOnly).Where(file => _mediaExtentions.Contains(Path.GetExtension(file))))
		{
			mover.MoveFile(mediaFile, targetDir);
		}
	}
}