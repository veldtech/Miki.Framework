using System.Linq;

namespace Miki.Services.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Miki.Cache;
    using Miki.Framework.Commands.Localization.Models;
    using Miki.Framework.Commands.Localization.Models.Exceptions;
    using Miki.Localization;
    using Miki.Localization.Models;

    public class LocalizationService : ILocalizationService
    {
        private readonly DbContext context;
        private readonly ICacheClient cache;
        private readonly Config config;

        private readonly HashSet<Locale> localeSet = new HashSet<Locale>();

        public LocalizationService(
            DbContext context,
            ICacheClient cache = null,
            Config config = null)
        {
            this.context = context;
            this.cache = cache;
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

            var language = await context.Set<ChannelLanguage>().FindAsync(id);
            if(language == null)
            {
                context.Set<ChannelLanguage>()
                    .Add(new ChannelLanguage
                    {
                        EntityId = id,
                        Language = iso3
                    });
            }
            else
            {
                language.Language = iso3;
            }

            if (cache != null)
            {
                await cache.UpsertAsync(GetLanguageCacheKey(id), iso3, config.CacheLifetime);
            }

            await context.SaveChangesAsync();
        }

        private async ValueTask<string> FetchLanguageIsoAsync(long id)
        {
            string iso = null;
            if (cache != null)
            {
                iso = await cache.GetAsync<string>(GetLanguageCacheKey(id));
            }

            if(iso == null)
            {
                var language = await context.Set<ChannelLanguage>().FindAsync(id);
                iso = language?.Language ?? config.DefaultIso3;
                if (cache != null)
                {
                    await cache.UpsertAsync(
                        GetLanguageCacheKey(id),
                        iso,
                        config.CacheLifetime);
                }
            }
            return iso;
        }

        private bool IsValidIso(string iso3)
        {
            return IsValidIso(new Locale(iso3, null));
        }

        private bool IsValidIso(Locale locale)
        {
            return localeSet.Contains(locale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetLanguageCacheKey(long channelId)
            => $"miki:language:{channelId}";

        public class Config
        {
            /// <summary>
            /// Time until cache ejects unused objects.
            /// </summary>
            public TimeSpan CacheLifetime { get; set; } = new TimeSpan(1, 0, 0);

            /// <summary>
            /// Default language for when users do not have a locale set up.
            /// </summary>
            public string DefaultIso3 { get; set; } = "eng";
        }
    }
}
