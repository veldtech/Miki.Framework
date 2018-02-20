using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Miki.Framework.Extension
{
    public static class StringExtension
    {
        public static TimeSpan GetTimeFromString(this string value)
        {
            TimeSpan timeUntilReminder = new TimeSpan();

			var matches = Regex.Matches(value.ToLower(), @"(\d+)(\s+)?(y|w|d|h|m|s)");

			foreach(Match match in matches)
			{
				if(match.Success)
				{
					timeUntilReminder += GetTimeFromMatch(match);
				}
			}

			if (timeUntilReminder >= TimeSpan.MaxValue)
            {
                return new TimeSpan();
            }

            return timeUntilReminder;
        }

		private static TimeSpan GetTimeFromMatch(Match m)
		{
			string value = m.Value;

			char timeNotifier = value.Last();
			value = value.Remove(value.Length - 1);

			if(int.TryParse(value, out int result))
			{
				switch (timeNotifier)
				{
					case 'y':
						return new TimeSpan(365 * result, 0, 0, 0, 0);
					case 'w':
						return new TimeSpan(7 * result, 0, 0, 0, 0);
					case 'd':
						return new TimeSpan(result, 0, 0, 0, 0);
					case 'h':
						return new TimeSpan(result, 0, 0);
					case 'm':
						return new TimeSpan(0, result, 0);
					case 's':
						return new TimeSpan(0, 0, result);
				}
			}

			return new TimeSpan();
		}
	}
}