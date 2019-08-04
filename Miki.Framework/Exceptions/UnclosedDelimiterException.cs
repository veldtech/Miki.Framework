using Miki.Localization;
using Miki.Localization.Exceptions;

namespace Miki.Framework.Exceptions
{
	public class UnclosedDelimiterException : LocalizedException
	{
		private readonly string _delimiter;
		private readonly string _input;

		public override IResource LocaleResource => new LanguageResource("error_unclosed_delimiter", _delimiter, _input);

		public UnclosedDelimiterException(string delimiter, string input)
		{
			_delimiter = delimiter;
			_input = input;
		}
	}
}
