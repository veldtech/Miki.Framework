namespace Miki.Framework
{
    using System;
    using System.Text.RegularExpressions;
    public static class StringExtension
	{
		public static TimeSpan GetTimeFromString(this string value)
		{
			TimeSpan timeUntilReminder = new TimeSpan();

			var matches = Regex.Matches(value.ToLower(), @"(\+|\-)?(\d+)(\s+)?(y|w|d|h|m|s)");
            if (matches.Count == 0)
            {
                return default;
            }

			foreach(Match match in matches)
			{
				if(match.Success)
				{
					timeUntilReminder += GetTimeFromMatch(match.Value);
				}
			}
            return timeUntilReminder;
		}

		private static TimeSpan GetTimeFromMatch(ReadOnlySpan<char> value)
		{
			ReadOnlySpan<char> timeNotifier = value.Slice(value.Length - 1, 1);
			value = value.Slice(0, value.Length - 1);

            if (!int.TryParse(value, out int result))
            {
                return default;
            }

            switch(timeNotifier.ToString())
            {
                case "y":
                    return TimeSpan.FromDays(365 * result);

                case "w":
                    return TimeSpan.FromDays(7 * result);

                case "d":
                    return TimeSpan.FromDays(result);

                case "h":
                    return TimeSpan.FromHours(result);

                case "m":
                    return TimeSpan.FromMinutes(result);

                case "s":
                    return TimeSpan.FromSeconds(result);
            }
            return default;
		}
	}
}