using System.Text;
using DownloadOrganizer.Contracts;
using DownloadOrganizer.Parsing;

namespace DownloadOrganizer;


public class MediaMover(IFileSystem fileSystem)
{
	class MediaInfo
	{
		public string Name { get; init; }
		public int? Year { get; init; }
		public int? SeasonNumber { get; init; }
		public int? Episode { get; init; }
		public bool IsSeries { get; init; }
	}

	public string MoveFile(string fullPathToFile, string targetBasePath)
	{
		var info = GetMediaInfo(fullPathToFile);
		var fileName = Path.GetFileName(fullPathToFile);
		
		if(info is null)
		{
			return string.Empty;
		}

		var (mediaFolder, seasonFolder) = GetSubFolder(info, targetBasePath);

		if (info.IsSeries)
		{
			fileSystem.CreateDirectory(seasonFolder);
			fileSystem.MoveFile(fullPathToFile, Path.Combine(seasonFolder, fileName));
		}
		else
		{
			fileSystem.CreateDirectory(mediaFolder);
			fileSystem.MoveFile(fullPathToFile, Path.Combine(mediaFolder, fileName));
		}

		return info.IsSeries ? seasonFolder : mediaFolder;
	}

	public string MoveFolder(string fullPathToFolder, string targetBasePath)
	{
		var info = GetMediaInfo(fullPathToFolder);

		if(info is null)
		{
			return string.Empty;
		}

		var (mediaFolder, seasonFolder) = GetSubFolder(info, targetBasePath);

		if (info.IsSeries)
		{
			fileSystem.MoveFolder(fullPathToFolder, seasonFolder);
		}
		else
		{
			fileSystem.MoveFolder(fullPathToFolder, mediaFolder);
		}
		
		return info.IsSeries ? seasonFolder : mediaFolder;
	}

	private MediaInfo GetMediaInfo(string fullPath)
	{
		var name = Path.GetFileName(fullPath);

		if(name is null)
		{
			return null;
		}

		var mkvFiles = fileSystem.GetFiles(fullPath, "*.mkv", SearchOption.AllDirectories);
		var mp4Files = fileSystem.GetFiles(fullPath, "*.mpv", SearchOption.AllDirectories);
		var details = TorrentNameParser.Parse(name);
		
		return new MediaInfo
		{
			Name = details.Title,
			Year = details.Year,
			SeasonNumber = details.Season,
			Episode = details.Episode,
			IsSeries = mkvFiles.Length > 1 || mp4Files.Length > 1 || details.Season > 0
		};
	}

	private (string MediaFolder, string SeasonFolder) GetSubFolder(MediaInfo info, string targetBasePath)
	{
		var sb = new StringBuilder();

		sb.Append(info.Name);
		if (info.Year > 0)
		{
			sb.Append($" ({info.Year})");
		}
		var newFolderName = sb.ToString();

		var subDirectory = info.IsSeries ? "Series" : "Movies";
		var mediaFolder = Path.Combine(targetBasePath, subDirectory, newFolderName);
		var seasonFolder = string.Empty;


		if (info.IsSeries)
		{
			var seriesDirectory = Path.Combine(targetBasePath, subDirectory);
			if(fileSystem.DirectoryExists(seriesDirectory))
			{
				var similarDirectories = fileSystem.GetDirectories(Path.Combine(targetBasePath, subDirectory), $"{info.Name} (*)");
				if (similarDirectories is { Length: 1 })
				{
					mediaFolder = similarDirectories[0];
				}
			}

			seasonFolder = Path.Combine(mediaFolder, $"Season {info.SeasonNumber}");
		}

		return (mediaFolder, seasonFolder);
	}
}