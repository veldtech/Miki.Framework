using Miki.Cache;
using Miki.Discord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Commands
{
	public abstract class CommandHandler
	{
		public Func<CommandEvent, IDiscordMessage, long, Task> OnMessageProcessed;

		public List<PrefixInstance> Prefixes { get; private set; } = new List<PrefixInstance>();

		protected ICachePool _cachePool = null;

		protected CommandMap _map = new CommandMap();

		public CommandHandler(ICachePool cachePool)
		{
			_cachePool = cachePool;
		}

		public void AddCommand(CommandEvent e)
		{
			_map.AddCommand(e);
		}

		public void AddModule(Module module)
		{
			_map.AddModule(module);
		}

		public void AddPrefix(string prefix, bool? isDefault = null, bool changable = false)
		{
			Prefixes.Add(new PrefixInstance(
				prefix,
				isDefault ?? !Prefixes.Any(x => x.IsDefault),
				changable,
				false
			));
		}

		public abstract Task CheckAsync(EventContext message);

		public PrefixInstance GetDefaultPrefix()
		{
			return Prefixes.FirstOrDefault(x => x.IsDefault);
		}

		public async Task<string> GetDefaultPrefixValueAsync(ulong guildId)
		{
			return await GetDefaultPrefix().GetForGuildAsync(await _cachePool.GetAsync(), guildId);
		}

		public void RemoveCommand(string commandName)
		{
			var command = _map.Commands.FirstOrDefault(x => x.Name == commandName.ToLower());

			if (command == null)
			{
				throw new ArgumentNullException();
			}

			_map.RemoveCommand(command);
		}

		public void RemoveCommand(CommandEvent e)
		{
			_map.RemoveCommand(e);
		}
	}
}