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

            return (timeNotifier.ToString()) switch
            {
                "y" => TimeSpan.FromDays(365 * result),
                "w" => TimeSpan.FromDays(7 * result),
                "d" => TimeSpan.FromDays(result),
                "h" => TimeSpan.FromHours(result),
                "m" => TimeSpan.FromMinutes(result),
                "s" => TimeSpan.FromSeconds(result),
                _ => default,
            };
        }
	}
}