using Miki.Framework.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Exceptions
{
	public class PermissionInsufficientException : CommandException
	{
		public override string Resource => "error_permission_insufficient";

		public PermissionInsufficientException(CommandEvent e) : base(e)
		{ }
	}
}
