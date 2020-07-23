using Miki.Localization;
using Miki.Localization.Exceptions;

namespace Miki.Framework.Exceptions
{
    class CommandNullException : LocalizedException
	{
		private readonly string command;

		public override IResource LocaleResource => new LanguageResource("error_command_null", command);

		public CommandNullException(string commandName)
		{
			command = commandName;
		}
	}
}