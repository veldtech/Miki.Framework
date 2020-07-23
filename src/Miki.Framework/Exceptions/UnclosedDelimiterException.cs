using Miki.Localization;
using Miki.Localization.Exceptions;

namespace Miki.Framework.Exceptions
{
    public class UnclosedDelimiterException : LocalizedException
	{
		private readonly string delimiter;
		private readonly string input;

		public override IResource LocaleResource 
            => new LanguageResource("error_unclosed_delimiter", delimiter, input);

		public UnclosedDelimiterException(string delimiter, string input)
		{
			this.delimiter = delimiter;
			this.input = input;
		}
	}
}
