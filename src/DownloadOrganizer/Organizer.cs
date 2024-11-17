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
			appSettings = JsonSerializer.Deserialize(File.ReadAllText(path), AppJsonSerializerContext.Default.AppSettings);
		}

		return appSettings;
	}

	public static async Task Organize()
	{
		var appSettings = GetAppSettings();
		await Organize(appSettings.DownloadsDirectory, appSettings.MediaDirectory);
	}

	public static async Task Organize(string sourceDir, string targetDir)
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
				await mover.MoveFolder(mediaFolder, targetDir);
			}
		}
		foreach (var mediaFile in fileSystem.GetFiles(sourceDir, "*.*", SearchOption.TopDirectoryOnly))
		{
			if(!FileExtensions.MediaFiles.Contains(Path.GetExtension(mediaFile)))
			{
				continue;
			}

			Log.WriteLog($"Found media file : {mediaFile}");
			await mover.MoveFile(mediaFile, targetDir);
		}
	}

	public static async Task Organize(string completedDownload)
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
			if (fileSystem.GetDirectories(extracted) is { Length: 1 } extractedSubDirs && fileSystem.GetFiles(extracted) is { Length: 0 })
			{
				var innerDirectory = extractedSubDirs[0];
				await mover.MoveFolder(innerDirectory, appSettings.MediaDirectory);
				fileSystem.DeleteDirectory(extracted);
			}
			else
			{
				await mover.MoveFolder(extracted, appSettings.MediaDirectory);
			}
		}
		else
		{
			var dirName = Path.GetDirectoryName(completedDownload);
			if (string.IsNullOrEmpty(dirName))
			{
				return;
			}
			
			var dir = Path.GetFullPath(dirName);
			var downloadDir = Path.GetFullPath(appSettings.DownloadsDirectory);

			if(dir == downloadDir)
			{
				if(FileExtensions.MediaFiles.Contains(ext))
				{
					await mover.MoveFile(completedDownload, appSettings.MediaDirectory);
				}
			}
			else
			{
				Log.WriteLog($"Moving : {dir}");
				await mover.MoveFolder(dir, appSettings.MediaDirectory);
			}
		}

	}
}