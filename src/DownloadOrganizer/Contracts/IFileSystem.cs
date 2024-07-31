namespace DownloadOrganizer.Contracts;

public interface IFileSystem
{
    string[] GetFiles(string directory);
    string[] GetFiles(string directory, string searchPattern);
    string[] GetFiles(string directory, string searchPattern, SearchOption searchOption);
    string[] GetDirectories(string directory);
    string[] GetDirectories(string directory, string searchPattern);
    void CreateDirectory(string directory);
    void MoveFile(string source, string destination);
    bool DirectoryExists(string directory);
    void DeleteDirectory(string directory);
    void DeleteFile(string file);
    void MoveFolder(string sourceFolder, string destFolder);
}