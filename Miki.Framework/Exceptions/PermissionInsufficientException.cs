using Miki.Framework.Events;
using Miki.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Exceptions
{
	public class PermissionInsufficientException : CommandException
	{
		public override IResource LocaleResource
			=> new LanguageResource("error_permission_insufficient");

		public PermissionInsufficientException(CommandEvent e) : base(e)
		{ }
	}
}
