using Miki.Logging;
using System;

namespace Miki.Framework.Services
{
	public class LoggingService
	{
		LogLevel _logLevel;

		public LoggingService(LogLevel defaultLevel = LogLevel.Information)
		{
			_logLevel = defaultLevel;
			new LogBuilder()
				.AddLogEvent((msg, level) => OnLog(msg, level))
				.SetLogHeader((level) => $"[{DateTime.UtcNow.ToShortTimeString()}]")
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
			_logLevel = level;
		}

		private void OnLog(string message, LogLevel level)
		{
			if(level >= _logLevel)
			{
				Console.WriteLine(message);
			}
		}
	}
}
