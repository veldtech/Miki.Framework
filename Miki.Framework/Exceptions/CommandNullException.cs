using Miki.Localization;
using Miki.Localization.Exceptions;

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