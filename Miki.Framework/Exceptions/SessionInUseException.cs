using Miki.Localization;
using Miki.Localization.Exceptions;

namespace Miki.Framework.Exceptions
{
	public class SessionInUseException : LocalizedException
	{
		public override IResource LocaleResource
			=> new LanguageResource("error_session_in_use");
	}
}