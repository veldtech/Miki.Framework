namespace Miki.Framework.Commands.Localization
{
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using Miki.Localization;
    using System;
    using System.Threading.Tasks;
    using Miki.Framework.Commands.Localization.Services;
    using Miki.Localization.Models;

    public class LocalizationPipelineStage : IPipelineStage
    {
        public const string LocaleContextKey = "framework-localization";

        private readonly ILocalizationService service;

        public LocalizationPipelineStage(ILocalizationService service)
        {
            this.service = service;
        }

        public async ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, Func<ValueTask> next)
        {
            var channel = e.GetChannel();
            
            if(channel == null)
            {
                // TODO: add GetDefaultLocale to ILocalizationService.
                if(!(service is LocalizationService extService))
                {
                    throw new NotSupportedException("Cannot fetch default locale from service");
                }

                var locale = extService.GetDefaultLocale();
                e.SetContext(LocaleContextKey, locale);
            }
            else
            {
                var locale = await service.GetLocaleAsync((long) channel.Id);
                e.SetContext(LocaleContextKey, locale);
            }

            await next();
        }
    }
}