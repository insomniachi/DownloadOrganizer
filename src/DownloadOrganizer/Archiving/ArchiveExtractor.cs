using DownloadOrganizer.Contracts;

namespace DownloadOrganizer.Archiving;

public class ArchiveExtractor(IFileSystem fileSystem, IEnumerable<IArchiver> archivers)
{
    public void ExtractAllArchives(string sourceDir)
    {
        foreach (var archiver in archivers)
        {
            foreach (var compressedFile in fileSystem.GetFiles(sourceDir, $"*.{archiver.Extension}"))
            {
                var extractedFolder = compressedFile.Replace($".{archiver.Extension}", "");
                archiver.ExtractToDirectory(compressedFile, extractedFolder);

                if (fileSystem.GetDirectories(extractedFolder) is { Length: 1 } extractedSubDirs && fileSystem.GetFiles(extractedFolder) is { Length: 0 })
                {
                    var innerDirectory = extractedSubDirs[0];
                    foreach (var item in fileSystem.GetFiles(innerDirectory))
                    {
                        var fileName = Path.GetFileName(item);
                        fileSystem.MoveFile(item, Path.Combine(extractedFolder, fileName));
                    }
                    foreach (var item in fileSystem.GetDirectories(innerDirectory))
                    {
                        var dirName = Path.GetFileName(item);
                        fileSystem.MoveFolder(item, Path.Combine(extractedFolder, dirName));
                    }
                    fileSystem.DeleteDirectory(innerDirectory);
                }

                fileSystem.DeleteFile(compressedFile);
            }
        }
    }
}
