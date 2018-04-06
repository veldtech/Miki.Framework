using Discord;
using Miki.Common;
using Miki.Framework.Exceptions;
using Miki.Framework.Languages;
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

		public async Task Check(IMessage e, CommandHandler c, string identifier = "")
		{
			string command = e.Content.Substring(identifier.Length).Split(' ')[0];
			string args = "";
			List<string> allAliases = new List<string>();
			List<string> arguments = new List<string>();

			if (e.Content.Split(' ').Length > 1)
			{
				args = e.Content.Substring(e.Content.Split(' ')[0].Length + 1);
				arguments.AddRange(args.Split(' '));
				arguments = arguments
					.Where(x => !string.IsNullOrWhiteSpace(x))
					.ToList();
			}

			if (Module != null)
			{
				if (Module.Nsfw && !(e.Channel as ITextChannel).IsNsfw)
				{
					throw new ChannelNotNsfwException();
				}
			}

			if (Aliases != null)
			{
				allAliases.AddRange(Aliases);
				allAliases.Add(Name);
			}

			if (IsOnCooldown(e.Author.Id))
			{
				Log.WarningAt(Name, " is on cooldown");
				return;
			}

			if (GuildPermissions.Count > 0)
			{
				foreach (GuildPermission g in GuildPermissions)
				{
					if (!(e.Author as IGuildUser).GuildPermissions.Has(g))
					{
						await e.Channel.SendMessageAsync($"Please give me the guild permission `{g}` to use this command.");
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

			Stopwatch sw = new Stopwatch();
			sw.Start();

			EventContext context = new EventContext();
			context.commandHandler = c;
			context.Arguments = new Args(args);
			context.message = e;
			context.EventSystem = eventSystem ?? EventSystem.Instance;

			bool success = await TryProcessCommand(targetCommand, context);
			await eventSystem.OnCommandDone(e, this, success, sw.ElapsedMilliseconds);

			if (success)
			{
				long elapsedMilliseconds = sw.ElapsedMilliseconds;
				TimesUsed++;
				Log.Message($"{Name} called by {e.Author.Username}#{e.Author.Discriminator} [{e.Author.Id}] from {(e.Channel as IGuildChannel).Guild.Name} in {elapsedMilliseconds}ms (+events: {sw.ElapsedMilliseconds}ms)");
			}

			sw.Stop();
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

		private async Task<bool> TryProcessCommand(ProcessCommandDelegate cmd, EventContext context)
		{
			bool success = false;

			try
			{
				await cmd(context);
				success = true;
			}
			catch (BotException botex)
			{
				await context.Channel.SendMessageAsync("", false, 
					new EmbedBuilder()
					{
						Title = $"🚫 {Locale.GetString(context.Channel.Id, LocaleTags.ErrorMessageGeneric)}",
						Description = Locale.GetString(context.Channel.Id, botex.Resource),
						Color = new Color(255, 0, 0)
					}.Build()
				);
				await eventSystem.OnCommandError(botex, this, context.message);
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				await context.Channel.SendMessageAsync("", false,
					new EmbedBuilder()
					{
						Title = $"🚫 {Locale.GetString(context.Channel.Id, LocaleTags.ErrorMessageGeneric)}",
						Description = ex.Message,
						Color = new Color(255, 0, 0)
					}.Build()
				);
				await eventSystem.OnCommandError(ex, this, context.message);
			}
			return success;
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