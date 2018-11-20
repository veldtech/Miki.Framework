using Miki.Localization;
using Miki.Localization.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Exceptions
{
	class CommandNullException : LocalizedException
	{
		private readonly string _command;

		public override IResource LocaleResource => new LanguageResource("error_command_null", _command);

		public CommandNullException(string commandName)
		{
			_command = commandName;
		}
	}
}