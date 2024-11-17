namespace DownloadOrganizer
{
	public static class Log
	{

#if DEBUG
		private static readonly ILogger Logger = new Logger();
#else
		private static readonly ILogger Logger = new FileLogger();
#endif

		public static void WriteLog(string msg) => Logger.WriteLine(msg);
	}

	public class FileLogger : ILogger
	{
		private static readonly string LogPath = Path.Combine(Organizer.GetAppSettings().Log);
		public void WriteLine(string msg) => File.AppendAllLines(LogPath, [msg]);
	}

	public class Logger : ILogger
	{
		public void WriteLine(string msg) => Console.WriteLine(msg);
	}

	public interface ILogger
	{
		void WriteLine(string msg);
	}
}
