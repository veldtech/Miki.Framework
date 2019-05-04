using Miki.Localization;

namespace Miki.Framework.Language
{
	public static class LanguageExtensions
	{
		public static LanguageResource CreateResource(this IContext context, string resource, params object[] parameters)
			=> new LanguageResource(resource, parameters);
	}
}