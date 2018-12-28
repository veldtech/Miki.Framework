using Microsoft.EntityFrameworkCore;
using Miki.Cache;
using Miki.Framework.Models;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
	public class PrefixInstance
	{
		public short Id { get; internal set; }
		public string Value { get; internal set; }
		public string DefaultValue { get; internal set; }

		public bool Changable { get; internal set; }
		public bool ForceCommandExecution { get; internal set; }

		public bool IsDefault { get; internal set; }

		internal PrefixInstance(string value, bool isDefault, bool changable = false, bool forceExec = false)
		{
			Value = value;
			DefaultValue = value;
			IsDefault = isDefault;
			Changable = changable;
			ForceCommandExecution = forceExec;
		}

		public async Task ChangeForGuildAsync(DbContext context, ICacheClient cache, ulong id, string prefix)
		{
			if (Changable)
			{
				long guildId = id.ToDbLong();

				Identifier i = await context.Set<Identifier>().FindAsync(guildId, DefaultValue);
				if (i == null)
				{
					await context.Set<Identifier>()
						.AddAsync(new Identifier() { GuildId = guildId, DefaultValue = DefaultValue, Value = prefix });
				}
				else
				{
					i.Value = prefix;
				}
				await context.SaveChangesAsync();

				await cache.UpsertAsync(GetCacheKey(id), prefix);
			}
		}

		private async Task<Identifier> CreateNewAsync(DbContext context, long id)
		{
			return (await context.Set<Identifier>()
				.AddAsync(new Identifier() { GuildId = id, DefaultValue = DefaultValue, Value = DefaultValue })).Entity;
		}

		private string GetCacheKey(ulong id)
			=> $"framework:prefix:{id}";

		public async Task<string> GetForGuildAsync(ICacheClient cache, ulong id)
		{
			if (Changable)
			{
				if (await cache.ExistsAsync(GetCacheKey(id)))
				{
					return await cache.GetAsync<string>(GetCacheKey(id));
				}
				string dbPrefix = await LoadFromDatabase(id);
				await cache.UpsertAsync(GetCacheKey(id), dbPrefix);
				return dbPrefix;
			}
			return DefaultValue;
		}

		public async Task<string> LoadFromDatabase(ulong id)
		{
			long guildId = id.ToDbLong();
			Identifier identifier = null;

			using (var context = MikiApp.Instance.GetService<DbContext>())
			{
				identifier = await context.Set<Identifier>().FindAsync(guildId, DefaultValue);
				if (identifier == null)
				{
					identifier = await CreateNewAsync(context, guildId);
				}

				await context.SaveChangesAsync();
			}

			return identifier.Value;
		}
	}
}