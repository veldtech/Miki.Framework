using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Miki.Cache;
using Miki.Discord.Common;
using Miki.Framework.Commands.Localization;
using Miki.Framework.Commands.Localization.Models;
using Miki.Framework.Commands.Pipelines;
using Miki.Localization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Localization
{
	public class LocalizationPipelineStage : IPipelineStage
	{
		public Dictionary<string, string> LocaleNames = new Dictionary<string, string>();
		/*private readonly Dictionary<string, string> _compatibilityLayer = new Dictionary<string, string>()
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
        };*/
		private string _defaultLocale = null;

		internal const string LocaleContext = "framework-localization";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private string GetCacheKeyForId(long channelId)
			=> $"miki:language:{channelId}";

		public async Task CheckAsync(IDiscordMessage data, IMutableContext e, Func<Task> next)
		{
			var db = e.GetService<DbContext>();
			if(db == null)
			{
				return;
			}

			var channel = e.GetChannel();
			if(channel == null)
			{
				throw new InvalidOperationException("Cannot set localization if no channel context is set.");
			}

			var locale = await GetLocaleAsync(e.Services, channel.Id);
			e.SetContext(LocaleContext, locale);

			await next();
		}

		public async Task<IResourceManager> GetLocaleAsync(IServiceProvider context, ulong channelId)
			=> await GetLocaleAsync(context, (long)channelId);
		public async Task<IResourceManager> GetLocaleAsync(IServiceProvider context, long channelId)
		{
			var cache = context.GetService<ICacheClient>();
			if(cache != null)
			{
				var cachedLang = await cache.GetAsync<string>(GetCacheKeyForId(channelId));
				if(cachedLang != null)
				{
					return LanguageDatabase.GetLanguageOrDefault(cachedLang);
				}
			}

			var db = context.GetService<DbContext>();
			ChannelLanguage l = await db.Set<ChannelLanguage>()
				.FindAsync(channelId);

			string lang = _defaultLocale;
			if(l != null)
			{
				lang = l.Language;
			}

			if(cache != null)
			{
				await cache.UpsertAsync(
					GetCacheKeyForId(channelId),
					lang,
					TimeSpan.FromDays(7));
			}
			return LanguageDatabase.GetLanguageOrDefault(lang);
		}

		public async Task SetLocaleForChannelAsync(IContext context, long channelId, string language)
		{
			var db = context.GetService<DbContext>();
			ChannelLanguage l = await db.Set<ChannelLanguage>()
				.FindAsync(channelId);

			if(l == null)
			{
				l = (await db.Set<ChannelLanguage>()
					.AddAsync(new ChannelLanguage()
					{
						EntityId = channelId,
						Language = language
					})).Entity;
			}
			else
			{
				l.Language = language;
			}

			var cache = context.GetService<ICacheClient>();
			if(cache != null)
			{
				await cache.UpsertAsync(
					GetCacheKeyForId(channelId),
					l.Language,
					TimeSpan.FromDays(7));
			}
			await db.SaveChangesAsync();
		}

		public string GetLocaleName(string input)
		{
			if(LocaleNames.TryGetValue(input, out var l))
			{
				return l;
			}
			return null;
		}

		public void LoadLanguage(string languageId, IResourceManager language, string localeName = null)
		{
			LanguageDatabase.AddLanguage(languageId, language);

			if(localeName != null)
			{
				LocaleNames.Add(localeName, languageId);
			}
		}

		public void SetDefaultLanguage(string iso)
		{
			LanguageDatabase.SetDefault(iso);
			_defaultLocale = iso;
		}
	}
}
namespace Miki.Framework.Commands
{
	public static class ContextExtensions
	{
		public static CommandPipelineBuilder UseLocalization(
			this CommandPipelineBuilder builder)
		{
			return builder.UseStage(new LocalizationPipelineStage());
		}

		public static IResourceManager GetLocale(this IContext context)
		{
			return context.GetContext<IResourceManager>(
				LocalizationPipelineStage.LocaleContext);
		}

		public static string GetString(this IResourceManager m, string key, params object[] format)
		{
			var str = m.GetString(key);
			return string.Format(str, format);
		}
	}
}
