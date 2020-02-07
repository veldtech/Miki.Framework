namespace Miki.Framework.Services
{
    using Miki.Logging;
    using System;

    public class LoggingService
	{
		private LogLevel logLevel;

		public LoggingService(LogLevel defaultLevel = LogLevel.Information)
		{
            logLevel = defaultLevel;
			new LogBuilder()
				.AddLogEvent(OnLog)
				.SetLogHeader(level => $"[{DateTime.UtcNow.ToShortTimeString()}]")
				.Apply();
		}
		public LoggingService(Action<LogBuilder> fn)
		{
			LogBuilder log = new LogBuilder();
			fn(log);
			log.Apply();
		}

		public void SetDefaultLogLevel(LogLevel level)
		{
            logLevel = level;
		}

		private void OnLog(string message, LogLevel level)
		{
			if(level >= logLevel)
			{
				Console.WriteLine(message);
			}
		}
	}
}
