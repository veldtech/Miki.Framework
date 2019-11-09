namespace Miki.Framework.Commands.Localization.Services
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Cache;
    using Framework;
    using Miki.Localization;
    using Miki.Localization.Models;
    using Models;
    using Models.Exceptions;
    using Patterns.Repositories;

    public class LocalizationService : ILocalizationService
    {
        private readonly IUnitOfWork context;
        private readonly IAsyncRepository<ChannelLanguage> repository;

        private readonly Config config;

        private readonly HashSet<Locale> localeSet = new HashSet<Locale>();

        public LocalizationService(
            IUnitOfWork context,
            Config config = null)
        {
            this.context = context;
            this.repository = context.GetRepository<ChannelLanguage>();
            this.config = config ?? new Config();
        }

        public void AddLocale(Locale locale)
        {
            localeSet.Add(locale);
        }

        public async ValueTask<Locale> GetLocaleAsync(long id)
        {
            var iso = await FetchLanguageIsoAsync(id);
            var localeRef = new Locale(iso, null);
            if(localeSet.TryGetValue(localeRef, out var locale))
            {
                return locale;
            }

            var defaultLocaleRef = new Locale(config.DefaultIso3, null);
            if(localeSet.TryGetValue(defaultLocaleRef, out var defaultLocale))
            {
                return defaultLocale;
            }
            throw new LocaleNotFoundException(iso);
        }

        public async IAsyncEnumerable<string> ListLocalesAsync()
        {
            await Task.Yield();
            foreach(var set in localeSet)
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

            var language = await repository.GetAsync(id);
            if(language == null)
            {
                await repository.AddAsync(
                    new ChannelLanguage
                    {
                        EntityId = id,
                        Language = iso3
                    });
            }
            else
            {
                language.Language = iso3;
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
            return localeSet.Contains(locale);
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
