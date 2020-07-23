using Miki.Localization;
using Miki.Localization.Exceptions;

namespace Miki.Framework.Exceptions
{
	/// <summary>
	/// An exception that gets thrown whenever a NSFW-only decorated command node gets used in a
	/// non-NSFW entity.
	/// </summary>
    public class ChannelNotNsfwException : LocalizedException
	{
		public override IResource LocaleResource
			=> new LanguageResource("error_channel_not_nsfw");
	}
}