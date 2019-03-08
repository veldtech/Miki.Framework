using Miki.Framework.Events;
using Miki.Localization;

namespace Miki.Framework.Language
{
	public static class LanguageExtensions
	{
		public static LanguageResource CreateResource(this EventContext context, string resource, params object[] parameters)
			=> new LanguageResource(resource, parameters);

		public static LocalizedEmbedBuilder CreateEmbedBuilder(this MessageContext context)
			=> new LocalizedEmbedBuilder(context.Locale);
	}
}