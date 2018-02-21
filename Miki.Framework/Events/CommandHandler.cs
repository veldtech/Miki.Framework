﻿using Miki.Common;
using Miki.Common.Events;
using Miki.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
    public class CommandHandlerBuilder
    {
        private CommandHandler commandHandler = null;

        public CommandHandlerBuilder(EventSystem eventSystem)
        {
            commandHandler = new CommandHandler(eventSystem);
        }

        public CommandHandlerBuilder AddCommand(ICommandEvent cmd)
        {
            commandHandler.AddCommand(cmd);
            return this;
        }

        public CommandHandlerBuilder AddModule(IModule module)
        {
            commandHandler.AddModule(module);
            return this;
        }

        public CommandHandlerBuilder SetOwner(IDiscordMessage owner)
        {
            commandHandler.IsPrivate = true;
            commandHandler.Owner = owner.Author.Id;
            commandHandler.ChannelId = owner.Channel.Id;
            return this;
        }

        public CommandHandlerBuilder DisposeInSeconds(int seconds)
        {
            commandHandler.ShouldBeDisposed = true;
            commandHandler.timeDisposed = DateTime.Now.AddSeconds(seconds);
            return this;
        }

        public CommandHandlerBuilder AddPrefix(string value)
        {
            if (!commandHandler.Prefixes.ContainsKey(value))
            {
                commandHandler.Prefixes.Add(value, new PrefixInstance(value, false, false));
            }
            return this;
        }

        public CommandHandler Build()
        {
            return commandHandler;
        }
    }

    public class CommandHandler : ICommandHandler
    {
		public List<ICommandEvent> Commands => commands.Values.ToList();

        public bool IsPrivate { get; set; } = false;

		public List<IModule> Modules => modules.Values.ToList();

        public bool ShouldBeDisposed { get; set; } = false;

        public ulong Owner { get; set; } = 0;
        public ulong ChannelId = 0;

		public async Task<string> GetPrefixAsync(ulong guildId)
			=> await Prefixes.FirstOrDefault(x => x.Value.IsDefault).Value.GetForGuildAsync(guildId);

        public DateTime TimeCreated = DateTime.Now;
        internal DateTime timeDisposed;

        internal EventSystem eventSystem;

        public Dictionary<string, PrefixInstance> Prefixes = new Dictionary<string, PrefixInstance>();

        internal Dictionary<string, string> Aliases = new Dictionary<string, string>();

        public Dictionary<string, IModule> modules = new Dictionary<string, IModule>();
        public Dictionary<string, ICommandEvent> commands = new Dictionary<string, ICommandEvent>();

        public CommandHandler(EventSystem eventSystem)
        {
            this.eventSystem = eventSystem;
        }

        public bool ShouldDispose()
        {
            return (DateTime.Now > timeDisposed);
        }

        public async Task CheckAsync(IDiscordMessage msg)
        {
            if (IsPrivate)
            {
                if (msg.Author.Id == Owner)
                {
                    foreach (PrefixInstance prefix in Prefixes.Values)
                    {
                        if (await TryRunCommandAsync(msg, prefix))
                        {
                            break;
                        }
                    }
                }
                return;
            }

            foreach (PrefixInstance prefix in Prefixes.Values)
            {
                if (await TryRunCommandAsync(msg, prefix))
                {
                    break;
                }
            }
        }

        public void AddCommand(ICommandEvent cmd)
        {
            foreach (string a in cmd.Aliases)
            {
                Aliases.Add(a, cmd.Name.ToLower());
            }
            commands.Add(cmd.Name.ToLower(), cmd);
        }

        public void AddModule(IModule module)
        {
            modules.Add(module.Name.ToLower(), module);
        }

        public async Task<bool> TryRunCommandAsync(IDiscordMessage msg, PrefixInstance prefix)
        {
            string identifier = await prefix.GetForGuildAsync(msg.Guild.Id);
            string message = msg.Content.ToLower();

            if (msg.Content.StartsWith(identifier))
            {
                message = Regex.Replace(message, @"\r\n?|\n", "");

				string command = message
					.Substring(identifier.Length)
					.Split(' ')
                    .First();

                command = (Aliases.ContainsKey(command)) ? Aliases[command] : command;

                ICommandEvent eventInstance = GetCommandEvent(command);

                if (eventInstance == null)
                {
                    return false;
                }

                if (GetUserAccessibility(msg) >= eventInstance.Accessibility)
                {
                    if (await eventInstance.IsEnabled(msg.Channel.Id) || prefix.ForceCommandExecution && GetUserAccessibility(msg) >= EventAccessibility.DEVELOPERONLY)
                    {
                        await eventInstance.Check(msg, this, identifier);
                        return true;
                    }
                }
                else
                {
                    await eventSystem.OnCommandDone(msg, eventInstance, false);
                }
            }
            return false;
        }

        public EventAccessibility GetUserAccessibility(IDiscordMessage e)
        {
            if (e.Channel == null) return EventAccessibility.PUBLIC;

            if (eventSystem.DeveloperIds.Contains(e.Author.Id)) return EventAccessibility.DEVELOPERONLY;
	    
            if (e.Author.HasPermissions(e.Channel, DiscordGuildPermission.Administrator) || 
	    	e.Author.HasPermissions(e.Channel, DiscordGuildPermission.ManageRoles)) return EventAccessibility.ADMINONLY;
	
            return EventAccessibility.PUBLIC;
        }

        public ICommandEvent GetCommandEvent(string value)
        {
            string newVal = value.ToLower();

            if(Aliases.ContainsKey(newVal))
            {
                return commands[Aliases[newVal]];
            }

            if (commands.ContainsKey(newVal))
            {
                return commands[newVal];
            }
            return null;
        }

        public IEvent GetEvent(string value)
        {
            foreach (IModule m in modules.Values)
            {
                IService s = m.Services.Where(x => x.Name.ToLower() == value.ToLower()).FirstOrDefault();
                if (s != null)
                {
                    return s;
                }
            }
            return GetCommandEvent(value);
        }

        public async Task RequestDisposeAsync()
        {
            if (Owner != 0)
            {
                await eventSystem.DisposePrivateCommandHandlerAsync(new Tuple<ulong, ulong>(Owner, ChannelId));
                return;
            }
            else
            {
                if (eventSystem.CommandHandler == this)
                {
                    Log.Warning("you just asked to dispose the standard command handler??");
                }
                else
                {
                    eventSystem.DisposeCommandHandler(this);
                }
            }
        }

        public IModule GetModule(string id)
        {
            if (modules.ContainsKey(id.ToLower()))
            {
                return modules[id.ToLower()];
            }
            return null;
        }

        public string[] GetAllEventNames()
        {
            List<string> allEvents = new List<string>();

            foreach (IModule m in modules.Values)
            {
                foreach (ICommandEvent c in m.Events)
                {
                    allEvents.Add(c.Name);
                    allEvents.AddRange(c.Aliases);
                }

                foreach (IService s in m.Services)
                {
                    allEvents.Add(s.Name);
                }
            }

            return allEvents.ToArray();
        }
    }
}
