using Miki.Framework;
using Miki.Framework.Models;
using Miki.Framework.Models.Context;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Resources;
using System.Threading.Tasks;

namespace Miki.Framework.Languages
{
    public static class Locale
    {
        public static Dictionary<string, ResourceManager> Locales = new Dictionary<string, ResourceManager>();

		// TODO: add resource based locale names
        public static Dictionary<string, string> LocaleNames = new Dictionary<string, string>();

        private const string defaultResource = "en-us";

		private const string defaultResult = "error_resource_missing";

		public static async Task<bool> HasStringAsync(ulong channelId, string m)
        {
			string lang = await GetLanguageAsync(channelId.ToDbLong());
			string output = Locales[lang].GetString(m);

			if (string.IsNullOrWhiteSpace(output))
			{
				output = Locales[defaultResource].GetString(m);
			}

			return !string.IsNullOrWhiteSpace(output);
		}

        public static async Task<string> GetStringAsync(ulong channelId, string m, params object[] p)
        {
			string language = await GetLanguageAsync(channelId.ToDbLong());
			ResourceManager resources = Locales[language];
			string output = null;

			if (InternalStringAvailable(m, resources))
			{
				output = InternalGetString(m, resources, p);

				if (string.IsNullOrWhiteSpace(output))
				{
					output = InternalGetString(m, Locales[defaultResource], p);
				}
			}
			else
			{
				output = InternalGetString(m, Locales[defaultResource], p);
			}

			return output ?? defaultResult;
		}

		public static async Task<string> GetLanguageAsync(long channelId)
		{
			var cache = Bot.Instance.CachePool.Get;
			var cacheKey = $"miki:language:{channelId}";

			if (await cache.ExistsAsync(cacheKey))
			{
				return await cache.GetAsync<string>(cacheKey);
			}
			else
			{
				using (var context = new IAContext())
				{
					ChannelLanguage l = context.Languages.Find(channelId);
					if (l != null)
					{
						await cache.UpsertAsync(cacheKey, l.Language);
						return l.Language;
					}
				}
			}
			await cache.UpsertAsync(cacheKey, defaultResource);
			return defaultResource;
		}

		public static void LoadLanguage(string languageId, string languageName, ResourceManager language)
		{
			Locales.Add(languageId, language);
			LocaleNames.Add(languageName, languageId);
		}

		public static async Task SetLanguageAsync(long id, string language)
		{
			var cache = Bot.Instance.CachePool.Get;
			var cacheKey = $"miki:language:{id}";

			using (var context = new IAContext())
			{
				ChannelLanguage lang = await context.Languages.FindAsync(id);

				if(LocaleNames.TryGetValue(language, out string val))
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
            return (p.Length == 0) ? lang.GetString(m) : string.Format(lang.GetString(m) ?? defaultResult, p); ;
        }
    }

	// TODO: shouldn't be here, remove or rework system.
    public class LocaleTags
    {
        public const string DisabledCommand = "miki_module_admin_disable_command";
        public const string DisabledModule = "miki_module_admin_disable_module";
        public const string EnabledCommand = "miki_module_admin_enable_command";
        public const string EnabledModule = "miki_module_admin_enable_module";
        public const string ErrorMessageGeneric = "miki_error_message_generic";
        public const string ErrorPickNoArgs = "miki_module_fun_pick_no_arg";
        public const string ImageNotFound = "miki_module_fun_image_error_no_image_found";
        public const string InsufficientMekos = "miki_mekos_insufficient";

		public const string JoinMessage = "miki_join_message";

        public const string PickMessage = "miki_module_fun_pick";

        public const string RollResult = "miki_module_fun_roll_result";

        public const string RouletteMessageNoArg = "miki_module_fun_roulette_winner_no_arg";
        public const string RouletteMessage = "miki_module_fun_roulette_winner";

        public const string SlotsHeader = "miki_module_fun_slots_header";

        public const string SlotsWinHeader = "miki_module_fun_slots_win_header";
        public const string SlotsWinMessage = "miki_module_fun_slots_win_amount";

        public const string SuccessMessageGeneric = "miki_success_message_generic";
    }
}