﻿using Microsoft.EntityFrameworkCore;
using Miki.Framework;
using Miki.Framework.Language;
using Miki.Framework.Models;
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
			{ "en-us", "eng" }
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
			}

			if(CompatibilityLayer.ContainsKey(resource))
			{
				resource = CompatibilityLayer[resource];
			}

			return new LocaleInstance(resource);
		}

		public static void LoadLanguage(string languageId, ResourceManager language)
		{
			LanguageDatabase.AddLanguage(languageId, language);
			// TODO: fix this
			//LocaleNames.Add(language.GetString(language, languageId.ThreeLetterISOLanguageName);
		}

		public static async Task SetLanguageAsync(DbContext context, ulong channelId, string language)
		{
			var cache = await Bot.Instance.CachePool.GetAsync();

			ChannelLanguage l = await context.Set<ChannelLanguage>().FindAsync(channelId.ToDbLong());

			if (l == null)
			{
				await context.Set<ChannelLanguage>().AddAsync(new ChannelLanguage()
				{
					EntityId = channelId.ToDbLong(),
					Language = language
				});
			}
			else
			{
				l.Language = language;
			}

			await context.SaveChangesAsync();
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