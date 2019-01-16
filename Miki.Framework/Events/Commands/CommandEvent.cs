using Miki.Discord.Common;
using Miki.Framework.Arguments;
using Miki.Framework.Exceptions;
using Miki.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
	public delegate Task ProcessCommandDelegate(EventContext context);

	public class CommandEvent : Event
	{
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
		public async Task Check(EventContext e, string identifier = "")
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
				if (Module.Nsfw && !e.Channel.IsNsfw)
				{
					throw new ChannelNotNsfwException();
				}
			}

			if (IsOnCooldown(e.message.Author.Id))
			{
				Log.Warning($"{Name} is on cooldown");
				return;
			}

			if (GuildPermissions.Count > 0)
			{
				foreach (GuildPermission g in GuildPermissions)
				{
                    var permissions = await e.Guild.GetPermissionsAsync(e.message.Author as IDiscordGuildUser);
                    if (!permissions.HasFlag(g))
					{
						await e.Channel.SendMessageAsync($"Please give me the guild permission `{g}` to use this command.");
						return;
					}
				}
			}

			ProcessCommandDelegate targetCommand = ProcessCommand;

            var argumentPack = new ArgumentPack(arguments);
            var provider = (ArgumentParseProvider)e.Services.GetService(typeof(ArgumentParseProvider));
            e.Arguments = new TypedArgumentPack(argumentPack, provider);
			
            await targetCommand(e);
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
				lastTimeUsed.TryAdd(id, new EventCooldownObject(Cooldown));
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