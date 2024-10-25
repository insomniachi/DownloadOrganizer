using DownloadOrganizer.Contracts;
using System.Runtime.InteropServices;

namespace DownloadOrganizer;

public class SystemIoFileSystem : IFileSystem
{
	public string[] GetFiles(string directory) => Directory.GetFiles(directory);
	public string[] GetFiles(string directory, string searchPattern) => Directory.GetFiles(directory, searchPattern);
	public string[] GetFiles(string directory, string searchPattern, SearchOption searchOption) => Directory.GetFiles(directory, searchPattern, searchOption);
	public string[] GetDirectories(string directory) => Directory.GetDirectories(directory);
	public string[] GetDirectories(string directory, string searchPattern) => Directory.GetDirectories(directory, searchPattern);
	public void CreateDirectory(string directory) => Directory.CreateDirectory(directory);
	public void MoveFile(string source, string destination) => File.Move(source, destination);
	public bool DirectoryExists(string directory) => Directory.Exists(directory);
	public void DeleteDirectory(string directory) => Directory.Delete(directory, true);
	public void DeleteFile(string file) => File.Delete(file);
	public void MoveFolder(string sourceFolder, string destFolder)
	{
		CreateDirectory(destFolder);
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			var directoryInfo = new DirectoryInfo(destFolder);
			directoryInfo.UnixFileMode = UnixFileMode.UserRead | UnixFileMode.UserWrite| UnixFileMode.UserExecute| UnixFileMode.GroupRead | UnixFileMode.GroupWrite | UnixFileMode.GroupExecute | UnixFileMode.OtherRead | UnixFileMode.OtherWrite | UnixFileMode.OtherExecute;
		}
		string[] files = GetFiles(sourceFolder);
		foreach (string file in files)
		{
			var ext = Path.GetExtension(file);
			if (FileExtensions.IgnoredFiles.Contains(ext))
			{
				DeleteFile(file);
				continue;
			}

			string name = Path.GetFileName(file);
			string dest = Path.Combine(destFolder, name);
			MoveFile(file, dest);
			if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				File.SetUnixFileMode(dest, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute | UnixFileMode.GroupRead | UnixFileMode.GroupWrite | UnixFileMode.GroupExecute | UnixFileMode.OtherRead | UnixFileMode.OtherWrite | UnixFileMode.OtherExecute);
			}
		}
		string[] folders = GetDirectories(sourceFolder);
		foreach (string folder in folders)
		{
			string name = Path.GetFileName(folder);
			string dest = Path.Combine(destFolder, name);
			MoveFolder(folder, dest);
		}
		DeleteDirectory(sourceFolder);
	}
}
