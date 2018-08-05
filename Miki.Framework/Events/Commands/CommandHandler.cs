using Miki.Discord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Commands
{
	public abstract class CommandHandler
	{
		public Func<CommandEvent, IDiscordMessage, long, Task> OnMessageProcessed;

		protected Dictionary<string, PrefixInstance> Prefixes { get; private set; } = new Dictionary<string, PrefixInstance>();

		protected CommandMap map = new CommandMap();

		public void AddCommand(CommandEvent e)
		{
			map.AddCommand(e);
		}

		public void AddModule(Module module)
		{
			map.AddModule(module);
		}

		public void AddPrefix(string prefix, bool? isDefault = null, bool changable = false)
		{
			Prefixes.Add(prefix, new PrefixInstance(
				prefix, 
				isDefault ?? !Prefixes.Any(x => x.Value.IsDefault), 
				changable, 
				false
			));
		}

		public abstract Task CheckAsync(EventContext message);

		public async Task<string> GetPrefixAsync(ulong guildId)
			=> await Prefixes.FirstOrDefault().Value.GetForGuildAsync(Bot.Instance.CachePool.Get, guildId);

		public void RemoveCommand(string commandName)
		{
			var command = map.Commands.FirstOrDefault(x => x.Name == commandName.ToLower());

			if(command == null)
			{
				throw new ArgumentNullException();
			}

			map.RemoveCommand(command);
		}
		public void RemoveCommand(CommandEvent e)
		{
			map.RemoveCommand(e);
		}
	}
}