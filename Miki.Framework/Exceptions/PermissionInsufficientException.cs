using Miki.Framework.Events;
using Miki.Localization;

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