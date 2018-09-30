using Miki.Framework.Events;
using Miki.Localization;
using Miki.Localization.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Exceptions
{
    public class CommandException : LocalizedException
	{
		public override IResource LocaleResource 
			=> new LanguageResource("error_default_command");

		public readonly CommandEvent Command;



		public CommandException(CommandEvent e) : base()
		{
			Command = e;
		}
	}
}
