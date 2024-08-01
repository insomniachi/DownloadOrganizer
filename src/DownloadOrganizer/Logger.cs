namespace DownloadOrganizer
{
	public static class Log
	{

#if DEBUG
		private static readonly ILogger _logger = new Logger();
#else
		private static readonly ILogger _logger = new FileLogger();
#endif

		public static void WriteLog(string msg) => _logger.WriteLine(msg);
	}

	public class FileLogger : ILogger
	{
		private static readonly string _logPath = Path.Combine(Organizer.GetAppSettings().Log);
		public void WriteLine(string msg) => File.AppendAllLines(_logPath, [msg]);
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
