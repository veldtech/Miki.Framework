namespace Miki.Framework.Commands.Localization.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Miki.Framework;
    using Miki.Localization;
    using Miki.Patterns.Repositories;
    using Models;
    using Models.Exceptions;

    public class LocalizationService : ILocalizationService
    {
        private readonly IUnitOfWork context;
        private readonly LocaleCollection locales;
        private readonly IAsyncRepository<ChannelLanguage> repository;

        private readonly Config config;

        public LocalizationService(
            IUnitOfWork context,
            LocaleCollection locales,
            Config config = null)
        {
            this.context = context;
            this.locales = locales;
            this.repository = context.GetRepository<ChannelLanguage>();
            this.config = config ?? new Config();
        }

        /// <inheritdoc />
        public void AddLocale(Locale locale)
        {
            throw new NotSupportedException("Pass locales through the LocaleCollection instead");
        }

        public Locale GetDefaultLocale()
        {
            return locales.Get(config.DefaultIso3);
        }

        public async ValueTask<Locale> GetLocaleAsync(long id)
        {
            var iso = await FetchLanguageIsoAsync(id);
            if(locales.TryGet(iso, out var locale))
            {
                return locale;
            }

            if(locales.TryGet(config.DefaultIso3, out var defaultLocale))
            {
                return defaultLocale;
            }
            throw new LocaleNotFoundException(iso);
        }

        public async IAsyncEnumerable<string> ListLocalesAsync()
        {
            await Task.Yield();
            foreach(var set in locales.List())
            {
                yield return set.CountryCode;
            }
        }

        public async ValueTask SetLocaleAsync(long id, string iso3)
        {
            if (!IsValidIso(iso3))
            {
                throw new LocaleNotFoundException(iso3);
            }

            var entity = new ChannelLanguage
            {
                EntityId = id,
                Language = iso3
            };

            var language = await repository.GetAsync(id);
            if(language == null)
            {
                await repository.AddAsync(entity);
            }
            else
            {
                await repository.EditAsync(entity);
            }
            await context.CommitAsync();
        }

        private async ValueTask<string> FetchLanguageIsoAsync(long id)
        {
            var language = await repository.GetAsync(id);
            return language?.Language ?? config.DefaultIso3;
        }

        private bool IsValidIso(string iso3)
        {
            return IsValidIso(new Locale(iso3, null));
        }

        private bool IsValidIso(Locale locale)
        {
            return locales.TryGet(locale.CountryCode, out _);
        }
        
        public class Config
        {
            /// <summary>
            /// Default language for when users do not have a locale set up.
            /// </summary>
            public string DefaultIso3 { get; set; } = "eng";
        }
    }
}
