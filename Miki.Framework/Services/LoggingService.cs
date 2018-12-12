using System;
using System.Collections.Generic;
using System.Text;
using Miki.Logging;

namespace Miki.Framework.Services
{
	public class LoggingService
	{
		LogLevel _logLevel;

		public LoggingService(LogLevel defaultLevel = LogLevel.Information)
		{
			_logLevel = defaultLevel;
		}

		public void AddBuffer(Action<string> fn)
		{
			if(fn == null)
			{
				throw new ArgumentNullException("fn");
			}

			Log.OnLog += (msg, lv) =>
			{
				if(lv >= _logLevel)
				{
					fn(msg);
				}
			};
		}

		public void SetDefaultLogLevel(LogLevel level)
		{
			_logLevel = level;
		}
	}
}
