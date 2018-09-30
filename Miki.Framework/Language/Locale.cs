using Microsoft.EntityFrameworkCore;
using Miki.Framework;
using Miki.Framework.Language;
using Miki.Framework.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading.Tasks;

namespace Miki.Framework.Languages
{
    public static class Locale
    {
		// TODO: add resource based locale names
        public static Dictionary<string, string> LocaleNames = new Dictionary<string, string>();
		public static Dictionary<string, string> CompatibilityLayer = new Dictionary<string, string>()
		{
			{ "uk-ae", "ara" },
			{ "bg-bg", "bul" },
			{ "cz-cz", "ces" },
			{ "da-dk", "dan" },
			{ "en-us", "eng" },
			{ "nl-nl", "dut" },
			{ "fi-fi", "fin" },
			{ "fr-fr", "fra" },
			{ "de-de", "ger" },
			{ "he-he", "heb" },
			{ "hi-hi", "hin" },
			{ "it-it", "ita" },
			{ "ja-ja", "jpn" },
			{ "lt-lt", "lit" },
			{ "ms-ms", "may" },
			{ "no-no", "nor" },
			{ "pl-pl", "pol" },
			{ "pt-pt", "por" },
			{ "pt-br", "por" },
			{ "ru-ru", "rus" },
			{ "es-es", "spa" },
			{ "sv-se", "swe" },
			{ "tl-ph", "tgl" },
			{ "uk-ua", "ukr" },
			{ "zh-chs", "zhs" },
			{ "zh-cht", "zht" },
		};

		public static string DefaultResource = "eng";

		public static async Task<LocaleInstance> GetLanguageInstanceAsync(ulong channelId)
		{
			var cache = await Bot.Instance.CachePool.GetAsync();
			var cacheKey = $"miki:language:{channelId}";

			string resource = null;

			if (await cache.ExistsAsync(cacheKey))
			{
				resource = await cache.GetAsync<string>(cacheKey);
			}
			else
			{
				using (var context = Bot.Instance.Information.DatabaseContextFactory())
				{
					ChannelLanguage l = await context.Set<ChannelLanguage>().FindAsync(channelId.ToDbLong());
					if (l != null)
					{
						await cache.UpsertAsync(cacheKey, l.Language);
						resource = l.Language;
					}
				}
			}

			if (resource == null)
			{
				await cache.UpsertAsync(cacheKey, DefaultResource);
				resource = DefaultResource;
			}
			else
			{
				if (CompatibilityLayer.ContainsKey(resource))
				{
					resource = CompatibilityLayer[resource];
				}
			}

			return new LocaleInstance(resource);
		}

		public static void LoadLanguage(string languageId, ResourceManager language, string localeName = null)
		{
			LanguageDatabase.AddLanguage(languageId, language);

			if (localeName != null)
			{
				LocaleNames.Add(localeName, languageId);
			}
		}

		public static void SetDefaultLanguage(string iso)
		{
			LanguageDatabase.SetDefault(iso);
		}

		public static async Task SetLanguageAsync(DbContext context, ulong channelId, string language)
		{
			var cache = await Bot.Instance.CachePool.GetAsync();
			var cacheKey = $"miki:language:{channelId}";

			ChannelLanguage l = await context.Set<ChannelLanguage>().FindAsync(channelId.ToDbLong());

			if (l == null)
			{
				l = (await context.Set<ChannelLanguage>().AddAsync(new ChannelLanguage()
				{
					EntityId = channelId.ToDbLong(),
					Language = language
				})).Entity;
			}
			else
			{
				l.Language = language;
			}

			await cache.UpsertAsync(cacheKey, l.Language);

			await context.SaveChangesAsync();
		}
	}
}