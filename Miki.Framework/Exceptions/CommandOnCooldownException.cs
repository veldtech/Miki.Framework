using Miki.Framework.Events;
using Miki.Localization;
using System;
using System.Collections.Generic;
using System.Text;

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
