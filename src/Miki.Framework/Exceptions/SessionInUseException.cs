namespace Miki.Framework.Exceptions
{
    using Miki.Localization.Exceptions;
    using Miki.Localization.Models;

    public class SessionInUseException : LocalizedException
	{
		public override IResource LocaleResource
			=> new LanguageResource("error_session_in_use");
	}
}