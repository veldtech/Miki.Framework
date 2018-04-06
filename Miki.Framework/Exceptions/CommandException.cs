using Miki.Framework.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Exceptions
{
    public class CommandException : BotException
	{
		public override string Resource => "error_default_command";
		public readonly CommandEvent Command;

		public CommandException(CommandEvent e) : base()
		{
			Command = e;
		}
	}
}
