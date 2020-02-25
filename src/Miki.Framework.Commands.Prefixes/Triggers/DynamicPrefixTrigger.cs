namespace Miki.Framework.Commands.Prefixes.Triggers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Miki.Cache;
    using Miki.Discord.Common;
    using Miki.Discord.Common.Models;
    using Miki.Framework.Commands.Prefixes.Models;

    public class DynamicPrefixTrigger : ITrigger
	{
		public string DefaultValue { get; internal set; }

		public Func<IContext, Task> OnTriggerReceived { get; set; }

		public DynamicPrefixTrigger(string value)
		{
			DefaultValue = value;
        }

        public async Task ChangeForGuildAsync(
            DbContext context,
            ICacheClient cache,
            ulong id,
            string prefix)
        {
            long guildId = id.ToDbLong();

            Prefix i = await context.Set<Prefix>()
                .FindAsync(guildId, DefaultValue);
            if(i == null)
            {
                await context.Set<Prefix>()
                    .AddAsync(new Prefix()
                    {
                        GuildId = guildId,
                        DefaultValue = DefaultValue,
                        Value = prefix
                    });
            }

            await context.SaveChangesAsync();
            await cache.UpsertAsync(GetCacheKey(id), prefix);
        }

        private async Task<Prefix> CreateNewAsync(DbContext context, long id)
        {
            return (await context.Set<Prefix>()
                .AddAsync(new Prefix()
                    {GuildId = id, DefaultValue = DefaultValue, Value = DefaultValue})).Entity;
        }

        private string GetCacheKey(ulong id)
			=> $"framework:prefix:{id}";

        public async Task<string> GetForGuildAsync(DbContext db, ICacheClient cache, ulong id)
        {
            if(await cache.ExistsAsync(GetCacheKey(id)))
            {
                return await cache.GetAsync<string>(GetCacheKey(id));
            }

            string dbPrefix = await LoadFromDatabase(db, id);
            await cache.UpsertAsync(GetCacheKey(id), dbPrefix);
            return dbPrefix;
        }

        public async Task<string> LoadFromDatabase(DbContext context, ulong id)
		{
			long guildId = id.ToDbLong();
			var identifier = await context.Set<Prefix>().FindAsync(guildId, DefaultValue);
            if(identifier != null)
            {
                return identifier.Value;
            }

            identifier = await CreateNewAsync(context, guildId);			
            await context.SaveChangesAsync();
            return identifier.Value;
		}

		public async Task<string> CheckTriggerAsync(IContext e)
        {
            var message = e.GetMessage();
			if(message == null)
			{
				return null;
			}

			var prefix = DefaultValue;
			if(message is IDiscordGuildMessage c)
			{
				prefix = await GetForGuildAsync(
					e.GetService<DbContext>(),
					e.GetService<ICacheClient>(),
					c.GuildId);
			}

			var query = e.GetQuery();
			if(!query.StartsWith(prefix))
			{
				return null;
			}

			if(OnTriggerReceived != null)
			{
				await OnTriggerReceived(e);
			}
			return prefix;
		}
	}
}