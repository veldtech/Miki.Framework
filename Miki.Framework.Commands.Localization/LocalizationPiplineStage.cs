namespace Miki.Framework.Commands.Localization
{
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using Miki.Localization;
    using System;
    using System.Threading.Tasks;

    public class LocalizationPipelineStage : IPipelineStage
	{
		public const string LocaleContext = "framework-localization";

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
				throw new InvalidOperationException("Cannot set localization if no channel context is set.");
			}

			var locale = await service.GetLocaleAsync((long)channel.Id);
			e.SetContext(LocaleContext, locale);

			await next();
		}
	}
}