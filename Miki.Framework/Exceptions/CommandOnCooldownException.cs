using Miki.Framework.Events;
using Miki.Localization;

namespace Miki.Framework.Exceptions
{
	public class CommandOnCooldownException : CommandException
	{
		public override IResource LocaleResource
			=> new LanguageResource("error_command_cooldown");

		public CommandOnCooldownException(CommandEvent e) : base(e)
		{ }
	}
}