namespace Miki.Framework.Exceptions
{
    using Miki.Localization;
    using Miki.Localization.Exceptions;
    using Miki.Localization.Models;

    public class ChannelNotNsfwException : LocalizedException
	{
		public override IResource LocaleResource
			=> new LanguageResource("error_channel_not_nsfw");
	}
}