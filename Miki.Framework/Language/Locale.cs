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

		private static ConcurrentDictionary<long, string> cache = new ConcurrentDictionary<long, string>();

        private static string defaultResource = "en-us";

		public static bool HasString(ulong channelId, string m)
        {
			string lang = GetLanguage(channelId.ToDbLong());
			string output = Locales[lang].GetString(m);

			if (string.IsNullOrWhiteSpace(output))
			{
				output = Locales[defaultResource].GetString(m);
			}

			return !string.IsNullOrWhiteSpace(output);
		}

        public static string GetString(ulong channelId, string m, params object[] p)
        {
			string language = GetLanguage(channelId.ToDbLong());
			ResourceManager resources = Locales[language];
			string output = "";

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

			return output;
		}

		public static string GetLanguage(long channelId)
		{
			if (cache.TryGetValue(channelId, out string language))
			{
				return language;
			}
			else
			{
				using (var context = new IAContext())
				{
					ChannelLanguage l = context.Languages.Find(channelId);
					if (l != null)
					{
						cache.TryAdd(channelId, l.Language);
						return l.Language;
					}
				}
			}
			return cache.GetOrAdd(channelId, defaultResource);
		}

		public static void LoadLanguage(string languageId, string languageName, ResourceManager language)
		{
			Locales.Add(languageId, language);
			LocaleNames.Add(languageName, languageId);
		}

		public static async Task SetLanguageAsync(long id, string language)
		{
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

				cache.AddOrUpdate(id, lang.Language, (x, y) => lang.Language);

				await context.SaveChangesAsync();
			}
		}

		private static bool InternalStringAvailable(string m, ResourceManager lang)
		{
			return lang.GetString(m) != null;
		}

        private static string InternalGetString(string m, ResourceManager lang, params object[] p)
        {
            return (p.Length == 0) ? lang.GetString(m) : string.Format(lang.GetString(m), p); ;
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