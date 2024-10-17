using System.IO.Compression;
using DownloadOrganizer.Contracts;

namespace DownloadOrganizer.Archiving;

public class ZipArchiver : IArchiver
{
    public string Extension { get; } = "zip";
    public void ExtractToDirectory(string zip, string destination)
    {
		using var archive = ZipFile.OpenRead(zip);
		foreach (var entry in archive.Entries)
		{
			entry.ExternalAttributes = 777;
		}

		archive.ExtractToDirectory(destination);
	}
}
