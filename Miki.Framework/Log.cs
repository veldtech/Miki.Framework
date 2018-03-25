using Microsoft.Extensions.Logging;
using Miki.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace Miki.Framework
{
	public class LogTheme
	{
		Dictionary<LogLevel, LogColor> colors = new Dictionary<LogLevel, LogColor>();

		public LogColor GetColor(LogLevel level)
		{
			if(colors.TryGetValue(level, out LogColor value))
			{
				return value;
			}
			return new LogColor();
		}

		public void SetColor(LogLevel level, LogColor color)
		{
			if(colors.ContainsKey(level))
			{
				colors[level] = color;
				return;
			}
			colors.Add(level, color);
		}
	}

	public struct LogColor
	{
		public ConsoleColor? Foreground;
		public ConsoleColor? Background;
	}

    public static class Log
    {
		public static event Action<string, LogLevel> OnLog;

		public static LogTheme Theme = new LogTheme();

        /// <summary>
        /// Display a [msg] message.
        /// </summary>
        /// <param name="message">information about the action</param>
        public static void Message(string message)
        {
			WriteToLog("[info] " + message, LogLevel.Information);
		}

		/// <summary>
		/// Display a error message.
		/// </summary>
		/// <param name="message">information about the action</param>
		public static void Error(string message)
		{
			WriteToLog("[crit] " + message, LogLevel.Error);
		}
		public static void Error(Exception exception)
		{
			Error("[crit]" + exception.ToString());
		}

        /// <summary>
        /// Display a warning message.
        /// </summary>
        /// <param name="message">information about the action</param>
        public static void Warning(string message)
        {
			WriteToLog("[warn] " + message, LogLevel.Warning);
        }

        /// <summary>
        /// Display a warning message.
        /// </summary>
        /// <param name="message">information about the action</param>
        public static void WarningAt(string tag, string message)
        {
			WriteToLog($"[warn@{tag}] {message}", LogLevel.Warning);
        }

		private static void WriteToLog(string message, LogLevel level)
		{
			LogColor color = Theme.GetColor(level);

			Console.ForegroundColor = color.Foreground ?? ConsoleColor.White;
			Console.BackgroundColor = color.Background ?? ConsoleColor.Black;

			OnLog?.Invoke(message, LogLevel.Information);

			Console.ResetColor();
		}
    }
}