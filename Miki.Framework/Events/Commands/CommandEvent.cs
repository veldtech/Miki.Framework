using Miki.Common;
using Miki.Discord.Common;
using Miki.Framework.Events.Commands;
using Miki.Framework.Exceptions;
using Miki.Framework.Languages;
using Miki.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
	public delegate Task ProcessCommandDelegate(EventContext context);

	public class CommandEvent : Event
	{
		public Dictionary<string, ProcessCommandDelegate> CommandPool { get; set; } = new Dictionary<string, ProcessCommandDelegate>();
		public int Cooldown { get; set; } = 3;

		public List<GuildPermission> GuildPermissions { get; set; } = new List<GuildPermission>();
		public string[] Aliases { get; set; } = new string[] { };

		public ProcessCommandDelegate ProcessCommand { get; set; } = async (context) => await Task.Delay(0);

		public CommandEvent()
		{
		}
		public CommandEvent(string name)
		{
			Name = name;
		}
		public CommandEvent(Action<CommandEvent> info)
		{
			info.Invoke(this);
		}

		// TODO: clean up
		public async Task Check(MessageContext e, string identifier = "")
		{
			string command = e.message.Content.Substring(identifier.Length).Split(' ')[0];
			string args = "";
			List<string> arguments = new List<string>();

			if (e.message.Content.Split(' ').Length > 1)
			{
				args = e.message.Content.Substring(e.message.Content.Split(' ')[0].Length + 1);
				arguments.AddRange(args.Split(' '));
				arguments = arguments
					.Where(x => !string.IsNullOrWhiteSpace(x))
					.ToList();
			}

			if (Module != null)
			{
				if (Module.Nsfw && !(await e.message.GetChannelAsync()).IsNsfw)
				{
					throw new ChannelNotNsfwException();
				}
			}

			if (IsOnCooldown(e.message.Author.Id))
			{
				Logging.Log.WarningAt(Name, " is on cooldown");
				return;
			}

			if (GuildPermissions.Count > 0)
			{
				foreach (GuildPermission g in GuildPermissions)
				{
					if (!(await (await e.message.GetChannelAsync() as IDiscordGuildChannel).GetPermissionsAsync(e.message.Author as IDiscordGuildUser)).HasFlag(g))
					{
						await (await e.message.GetChannelAsync()).SendMessageAsync($"Please give me the guild permission `{g}` to use this command.");
						return;
					}
				}
			}

			ProcessCommandDelegate targetCommand = ProcessCommand;

			if (arguments.Count > 0)
			{
				if (CommandPool.ContainsKey(arguments[0]))
				{
					targetCommand = CommandPool[arguments[0]];
					args = args.Substring((arguments[0].Length == args.Length) ? arguments[0].Length : arguments[0].Length + 1);
				}
			}

			EventContext context = new EventContext();
			context.message = e.message;
			context.Channel = e.channel;

			if(context.Channel is IDiscordGuildChannel c)
			{
				context.Guild = await c.GetGuildAsync();
			}

			context.commandHandler = e.commandHandler;
			context.Arguments = new Args(args);
			context.EventSystem = e.eventSystem;

			await targetCommand(context);
		}

		private bool IsOnCooldown(ulong id)
		{
			if (lastTimeUsed.ContainsKey(id))
			{
				if (lastTimeUsed[id].CanBeUsed())
				{
					lastTimeUsed[id].Tick();
					return false;
				}
				return true;
			}
			else
			{
				lastTimeUsed.Add(id, new EventCooldownObject(Cooldown));
				return false;
			}
		}

		public CommandEvent SetCooldown(int seconds)
		{
			Cooldown = seconds;
			return this;
		}

		public CommandEvent SetPermissions(params GuildPermission[] permissions)
		{
			GuildPermissions.AddRange(permissions);
			return this;
		}

		public CommandEvent Default(ProcessCommandDelegate command)
		{
			ProcessCommand = command;
			return this;
		}

		new public CommandEvent SetName(string name)
		{
			Name = name;
			return this;
		}

		new public CommandEvent SetAccessibility(EventAccessibility accessibility)
		{
			Accessibility = accessibility;
			return this;
		}

		public CommandEvent SetAliases(params string[] Aliases)
		{
			this.Aliases = Aliases;
			return this;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}