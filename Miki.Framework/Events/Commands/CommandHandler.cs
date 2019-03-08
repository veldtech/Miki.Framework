using Microsoft.EntityFrameworkCore;
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

		protected ICacheClient _cachePool = null;

		protected CommandMap _map = new CommandMap();

		public void AddCommand(CommandEvent e)
		{
			_map.AddCommand(e);
		}

		public void AddModule(Module module)
		{
			_map.AddModule(module);
		}

		public abstract Task CheckAsync(CommandContext message);

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