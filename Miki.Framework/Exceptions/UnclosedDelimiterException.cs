using Miki.Localization;
using Miki.Localization.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Exceptions
{
	public class UnclosedDelimiterException : LocalizedException
	{
		private string _delimiter;
		private string _input;

		public override IResource LocaleResource => new LanguageResource("error_unclosed_delimiter", _delimiter, _input);

		public UnclosedDelimiterException(string delimiter, string input)
		{
			_delimiter = delimiter;
			_input = input;
		}
	}
}
