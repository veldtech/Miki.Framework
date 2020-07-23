using System;
using System.Threading.Tasks;
using Miki.Cache;
using Miki.Discord.Common.Models;
using Miki.Framework.Commands.Prefixes.Models;
using Miki.Patterns.Repositories;

namespace Miki.Framework.Commands.Prefixes.Triggers
{
    public class DynamicPrefixTrigger : ITrigger
	{
		public string DefaultValue { get; internal set; }

		public Func<IContext, Task> OnTriggerReceived { get; set; }

		public DynamicPrefixTrigger(string value)
		{
			DefaultValue = value;
        }

        private string GetCacheKey(ulong id)
			=> $"framework:prefix:{id}";

        public async Task ChangeForGuildAsync(
            IUnitOfWork context,
            ICacheClient cache,
            ulong id,
            string prefix)
        {
            var repository = context.GetRepository<Prefix>();

            Prefix currentPrefix = await repository.GetAsync((long)id, DefaultValue);
            if(currentPrefix == null)
            {
                await repository.AddAsync(new Prefix
                {
                    GuildId = (long) id,
                    DefaultValue = DefaultValue,
                    Value = prefix
                });
            }
            else
            {
                currentPrefix.Value = prefix;
                await repository.EditAsync(currentPrefix);
            }

            await context.CommitAsync();
            await cache.UpsertAsync(GetCacheKey(id), prefix);
        }

        public async Task<string> GetForGuildAsync(IUnitOfWork db, ICacheClient cache, ulong id)
        {
            if(await cache.ExistsAsync(GetCacheKey(id)))
            {
                return await cache.GetAsync<string>(GetCacheKey(id));
            }

            string dbPrefix = await LoadFromDatabaseAsync(db, id);
            await cache.UpsertAsync(GetCacheKey(id), dbPrefix);
            return dbPrefix;
        }

        public async Task<string> LoadFromDatabaseAsync(IUnitOfWork context, ulong id)
        {
            var repository = context.GetRepository<Prefix>();

            var identifier = await repository.GetAsync((long)id, DefaultValue);
            if(identifier != null)
            {
                return identifier.Value;
            }

            identifier = await repository.AddAsync(new Prefix
            {
                GuildId = (long)id,
                DefaultValue = DefaultValue,
                Value = DefaultValue
            });
            await context.CommitAsync();
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
                    e.GetService<IUnitOfWork>(),
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