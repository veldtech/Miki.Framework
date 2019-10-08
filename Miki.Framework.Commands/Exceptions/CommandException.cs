namespace Miki.Framework.Commands
{
    using Miki.Localization.Exceptions;
    using Miki.Localization.Models;

    public class CommandException : LocalizedException
	{
		public override IResource LocaleResource
			=> new LanguageResource("error_default_command");

		public readonly Node Command;

		public CommandException(Node e) : base()
		{
			Command = e;
		}
	}
}