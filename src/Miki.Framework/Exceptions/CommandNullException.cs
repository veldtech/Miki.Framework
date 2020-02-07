namespace Miki.Framework.Exceptions
{
    using Miki.Localization.Exceptions;
    using Miki.Localization.Models;
    
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