using Miki.Framework.Languages;
using Miki.Framework.Models;
using Miki.Framework.Models.Context;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Language
{
    public class LocaleInstance
    {
		readonly string _language;

		public const string defaultResource = "en-us";

		private const string defaultResult = "error_resource_missing";

		public LocaleInstance(string language)
		{
			_language = language;
		}

		public string GetString(string m, params object[] p)
		{
			if (Locale.Locales.TryGetValue(_language, out var resources))
			{
				string output = null;

				if (InternalStringAvailable(m, resources))
				{
					output = InternalGetString(m, resources, p);

					if (string.IsNullOrWhiteSpace(output))
					{
						output = InternalGetString(m, Locale.Locales[defaultResource], p);
					}
				}
				else
				{
					output = InternalGetString(m, Locale.Locales[defaultResource], p);
				}

				return output ?? defaultResult;
			}
			return defaultResult;
		}


		public bool HasString(string m)
		{
			string output = Locale.Locales[_language].GetString(m);

			if (string.IsNullOrWhiteSpace(output))
			{
				output = Locale.Locales[defaultResource].GetString(m);
			}

			return !string.IsNullOrWhiteSpace(output);
		}


		public async Task SetLanguageAsync(long id, string language)
		{
			var cache = Bot.Instance.CachePool.Get;
			var cacheKey = $"miki:language:{id}";

			using (var context = new IAContext())
			{
				ChannelLanguage lang = await context.Languages.FindAsync(id);

				if (Locale.LocaleNames.TryGetValue(language, out string val))
				{
					language = val;
				}

				if (lang == null)
				{
					lang = context.Languages.Add(new ChannelLanguage()
					{
						EntityId = id,
						Language = language
					}).Entity;
				}

				lang.Language = language;

				await cache.UpsertAsync(cacheKey, lang.Language);
				await context.SaveChangesAsync();
			}
		}

		private static bool InternalStringAvailable(string m, ResourceManager lang)
		{
			return lang.GetString(m) != null;
		}

		private static string InternalGetString(string m, ResourceManager lang, params object[] p)
		{
			return (p.Length == 0) ? lang.GetString(m) : string.Format(lang.GetString(m) ?? defaultResult, p);
			;
		}
	}
}
