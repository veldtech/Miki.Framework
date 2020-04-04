namespace Miki.Framework.Exceptions
{
    using Miki.Localization;
    using Miki.Localization.Exceptions;

    public class SessionInUseException : LocalizedException
	{
		public override IResource LocaleResource
			=> new LanguageResource("error_session_in_use");
	}
}