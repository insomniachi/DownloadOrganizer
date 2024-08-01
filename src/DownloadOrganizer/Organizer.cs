using DownloadOrganizer.Archiving;
using System.Text.Json;

namespace DownloadOrganizer;

public static class Organizer
{
	public static AppSettings GetAppSettings()
	{
		var appSettings = new AppSettings();
		var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		var path = Path.Combine(appData, "DownloadOrganizer", "appsettings.json");
		if (File.Exists(path))
		{
			appSettings = JsonSerializer.Deserialize(File.ReadAllText(path), AppSettingsJsonSerializerContext.Default.AppSettings);
		}

		return appSettings;
	}

	public static void Organize()
	{
		var appSettings = GetAppSettings();
		Organize(appSettings.DownloadsDirectory, appSettings.MediaDirectory);
	}

	public static void Organize(string sourceDir, string targetDir)
	{
		Log.WriteLog($"Organizing {sourceDir}");

		var fileSystem = new SystemIoFileSystem();
		var mover = new MediaMover(fileSystem);
		var archiveExtractor = new ArchiveExtractor(fileSystem, [new ZipArchiver()]);

		archiveExtractor.ExtractAllArchives(sourceDir);

		foreach (var mediaFolder in fileSystem.GetDirectories(sourceDir))
		{
			Log.WriteLog($"Found media directory : {mediaFolder}");
			var files = fileSystem.GetFiles(mediaFolder);
			if (files.Any(x => FileExtensions.MediaFiles.Contains(Path.GetExtension(x))))
			{
				mover.MoveFolder(mediaFolder, targetDir);
			}
		}
		foreach (var mediaFile in fileSystem.GetFiles(sourceDir, "*.*", SearchOption.TopDirectoryOnly))
		{
			if(!FileExtensions.MediaFiles.Contains(Path.GetExtension(mediaFile)))
			{
				continue;
			}

			Log.WriteLog($"Found media file : {mediaFile}");
			mover.MoveFile(mediaFile, targetDir);
		}
	}

	public static void Organize(string completedDownload)
	{
		Log.WriteLog($"Organizing download : {completedDownload}");
		var fileSystem = new SystemIoFileSystem();
		var mover = new MediaMover(fileSystem);
		var ext = Path.GetExtension(completedDownload);
		var appSettings = GetAppSettings();

		if (ext == FileExtensions.Zip)
		{
			Log.WriteLog("extracting");
			var archiveExtractor = new ArchiveExtractor(fileSystem, [new ZipArchiver()]);
			var extracted = archiveExtractor.ExtractArchive(completedDownload);
			Log.WriteLog($"Moving : {extracted}");
			mover.MoveFolder(extracted, appSettings.MediaDirectory);
		}
		else
		{
			var dir = Path.GetFullPath(Path.GetDirectoryName(completedDownload));
			var downloadDir = Path.GetFullPath(appSettings.DownloadsDirectory);

			if(dir == downloadDir)
			{
				if(FileExtensions.MediaFiles.Contains(ext))
				{
					mover.MoveFile(completedDownload, appSettings.MediaDirectory);
				}
			}
			else
			{
				Log.WriteLog($"Moving : {dir}");
				mover.MoveFolder(dir, appSettings.MediaDirectory);
			}
		}

	}
}