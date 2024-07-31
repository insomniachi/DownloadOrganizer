namespace DownloadOrganizer.Contracts;

public interface IArchiver
{
    string Extension { get; }
    void ExtractToDirectory(string zip, string destionation);
}
