
namespace Miki.Framework.Commands
{
    using Microsoft.Extensions.DependencyInjection;
    using Miki.Framework.Commands.Localization;
    using Miki.Localization;

    public static class ContextExtensions
    {
        public static CommandPipelineBuilder UseLocalization(
            this CommandPipelineBuilder builder)
        {
            return builder.UseStage(
                new LocalizationPipelineStage(
                    builder.Services.GetRequiredService<ILocalizationService>()));
        }

        public static Locale GetLocale(this IContext context)
        {
            return context.GetContext<Locale>(
                LocalizationPipelineStage.LocaleContextKey);
        }

        public static string GetString(this IResourceManager m, string key, params object[] format)
        {
            var str = m.GetString(key);
            return string.Format(str, format);
        }
    }
}
