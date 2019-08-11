namespace Miki.Framework.Language
{
    using Miki.Localization;

    public static class LanguageExtensions
	{
		public static LanguageResource CreateResource(this IContext _, string resource, params object[] parameters)
			=> new LanguageResource(resource, parameters);
	}
}