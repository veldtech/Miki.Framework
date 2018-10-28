using Miki.Localization;
using Miki.Localization.Exceptions;

namespace Miki.Framework.Exceptions
{
	public class ChannelNotNsfwException : LocalizedException
	{
		public override IResource LocaleResource
			=> new LanguageResource("error_channel_not_nsfw");
	}
}