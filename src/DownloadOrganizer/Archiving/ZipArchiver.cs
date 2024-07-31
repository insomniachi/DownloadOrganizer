using System.IO.Compression;
using DownloadOrganizer.Contracts;

namespace DownloadOrganizer.Archiving;

public class ZipArchiver : IArchiver
{
    public string Extension { get; } = "zip";
    public void ExtractToDirectory(string zip, string destination) => ZipFile.ExtractToDirectory(zip, destination);
}
