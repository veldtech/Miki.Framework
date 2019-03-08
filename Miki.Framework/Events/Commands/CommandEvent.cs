using Miki.Discord.Common;
using Miki.Framework.Arguments;
using Miki.Framework.Events.Attributes;
using Miki.Framework.Events.Commands;
using Miki.Framework.Exceptions;
using Miki.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
	public delegate Task ProcessCommandDelegate(ICommandContext context);

	public class CommandEvent : Event, ICommand
	{
		public int Cooldown { get; set; } = 3;

        public List<CommandRequirementAttribute> Requirements = new List<CommandRequirementAttribute>();

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

		public async Task ExecuteAsync(CommandContext e)
		{
			List<string> arguments = new List<string>();
			if (e.Message.Content.Split(' ').Length > 1)
			{
				string args = e.Message.Content.Substring(e.Message.Content.Split(' ')[0].Length + 1);
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

			if (IsOnCooldown(e.Message.Author.Id))
			{
				Log.Warning($"{Name} is on cooldown");
				return;
			}

			if (GuildPermissions.Count > 0)
			{
				foreach (GuildPermission g in GuildPermissions)
				{
                    var permissions = await e.Guild.GetPermissionsAsync(e.Message.Author as IDiscordGuildUser);
                    if (!permissions.HasFlag(g))
					{
						await e.Channel.SendMessageAsync($"Please give me the guild permission `{g}` to use this command.");
						return;
					}
				}
			}

            var argumentPack = new ArgumentPack(arguments);
            var provider = (ArgumentParseProvider)e.Services.GetService(typeof(ArgumentParseProvider));
            e.Arguments = new TypedArgumentPack(argumentPack, provider);
			
            foreach(var r in Requirements)
            {
                if(!await r.CheckAsync(e))
                {
                    await r.OnCheckFail(e);
                    return;
                }
            }

            await ProcessCommand(e);
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

		public override string ToString()
		{
			return Name;
		}
	}
}