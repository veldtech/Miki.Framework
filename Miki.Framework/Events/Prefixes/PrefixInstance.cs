using Miki.Framework.Models;
using Miki.Framework.Models.Context;
using System;
using System.Collections.Concurrent;
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

		private ConcurrentDictionary<ulong, string> cache = new ConcurrentDictionary<ulong, string>();

		internal PrefixInstance(string value, bool isDefault, bool changable = false, bool forceExec = false)
		{
			Value = value;
			DefaultValue = value;
			Changable = changable;
			ForceCommandExecution = forceExec;
		}

		public async Task ChangeForGuildAsync(ulong id, string prefix)
		{
			if (Changable)
			{
				using (var context = new IAContext())
				{
					long guildId = id.ToDbLong();

					Identifier i = await context.Identifiers.FindAsync(guildId, DefaultValue);
					if (i == null)
					{
						context.Identifiers.Add(new Identifier() { GuildId = guildId, DefaultValue = DefaultValue, Value = prefix });
					}
					else
					{
						i.Value = prefix;
					}
					await context.SaveChangesAsync();
				}
				cache.AddOrUpdate(id, (x) => prefix, (x, y) => prefix);
			}
		}

		async Task<Identifier> CreateNewAsync(IAContext context, long id)
		{
			return (await context.Identifiers.AddAsync(new Identifier() { GuildId = id, DefaultValue = DefaultValue, Value = DefaultValue })).Entity;
		}

		public async Task<string> GetForGuildAsync(ulong id)
		{
			if (Changable)
			{
				if (cache.TryGetValue(id, out string value))
				{
					return value;
				}
				return cache.GetOrAdd(id, await LoadFromDatabase(id));
			}
			return DefaultValue;
		}

		public async Task<string> LoadFromDatabase(ulong id)
		{
			long guildId = id.ToDbLong();
			Identifier identifier = null;

			using (var context = new IAContext())
			{
				identifier = await context.Identifiers.FindAsync(guildId, DefaultValue);
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
 