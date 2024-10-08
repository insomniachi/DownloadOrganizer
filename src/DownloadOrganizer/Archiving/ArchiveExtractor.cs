﻿using DownloadOrganizer.Contracts;

namespace DownloadOrganizer.Archiving;

public class ArchiveExtractor(IFileSystem fileSystem, IEnumerable<IArchiver> archivers)
{
    public void ExtractAllArchives(string sourceDir)
    {
        foreach (var archiver in archivers)
        {
            foreach (var compressedFile in fileSystem.GetFiles(sourceDir, $"*.{archiver.Extension}"))
            {
				ExtractArchive(compressedFile, archiver);
			}
        }
    }

    public string ExtractArchive(string compressedFile, IArchiver archiver = null)
    {
        var ext = Path.GetExtension(compressedFile);
        archiver ??= archivers.FirstOrDefault(x => $".{x.Extension}" == ext);

        if(archiver is null)
        {
            return "";
        }

		var extractedFolder = compressedFile.Replace($".{archiver.Extension}", "");

		foreach (var item in FileExtensions.MediaFiles)
		{
			extractedFolder = extractedFolder.Replace(item, "");
		}

		archiver.ExtractToDirectory(compressedFile, extractedFolder);
		fileSystem.DeleteFile(compressedFile);
        return extractedFolder;
	}
}
