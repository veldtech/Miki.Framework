using Miki.Framework.Events.Attributes;
using Miki.Framework.Models;
using Miki.Framework.Models.Context;
using Miki.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Diagnostics;

namespace Miki.Framework.Events
{
    public class EventSystem
    {
        public static EventSystem Instance => _instance;
        private static EventSystem _instance = null;

        public delegate Task ExceptionDelegate(Exception ex, CommandEvent command, IMessage message);

		public Dictionary<string, CommandEvent> Commands 
			=> commandHandler.commands;

		public CommandHandler CommandHandler 
			=> commandHandler;

		public int CommandsUsed
			=> commandHandler.commands.Values
				.Sum(x => x.TimesUsed);

		public List<ulong> DeveloperIds => developers;

		public Dictionary<string, Module> Modules => commandHandler.modules;
		
		private List<ulong> developers = new List<ulong>();

		private Dictionary<ulong, OnRegisteredMessage> registeredUsers = new Dictionary<ulong, OnRegisteredMessage>();

		private CommandHandler commandHandler;

		private List<CommandHandler> commandHandlers = new List<CommandHandler>();
		
        private ConcurrentDictionary<Tuple<ulong, ulong>, CommandHandler> privateCommandHandlers = new ConcurrentDictionary<Tuple<ulong, ulong>, CommandHandler>();

		private ConcurrentDictionary<Tuple<ulong, ulong>, Action<IMessage>> nextMessageRequests = new ConcurrentDictionary<Tuple<ulong, ulong>, Action<IMessage>>();

		private Bot bot = null;

		private List<ulong> ignore = new List<ulong>();

		private EventContainer events = new EventContainer();

        public ExceptionDelegate OnCommandError = async (ex, command, msg) => await Task.Yield();

        private EventSystem(Bot bot)
        {
            if (this.bot != null)
            {
                Log.Warning("EventSystem already defined, terminating...");
                return;
            }

            this.bot = bot;

            commandHandler = new CommandHandler(this);

            bot.Client.MessageReceived += InternalMessageReceived;
        }

        public void AddCommandDoneEvent(Action<CommandDoneEvent> info)
        {
            CommandDoneEvent newEvent = new CommandDoneEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            if (newEvent.Aliases.Length > 0)
            {
                foreach (string s in newEvent.Aliases)
                {
                    commandHandler.Aliases.Add(s, newEvent.Name.ToLower());
                }
            }
            events.CommandDoneEvents.Add(newEvent.Name.ToLower(), newEvent);
        }

		public void AddDeveloper(ulong id)
			=> developers.Add(id);

        public int GetCommandsUsed(string eventName)
        {
            return CommandHandler.GetCommandEvent(eventName).TimesUsed;
        }

        internal void DisposeCommandHandler(CommandHandler commandHandler)
        {
            commandHandlers.Remove(commandHandler);
        }

		public void Ignore(ulong id)
		{
			ignore.Add(id);
		}

		public bool PrivateCommandHandlerExist(ulong userId, ulong channelId)
		{
			return privateCommandHandlers.ContainsKey(new Tuple<ulong, ulong>(userId, channelId));
		}

        internal async Task DisposePrivateCommandHandlerAsync(Tuple<ulong, ulong> key)
        {
            if(!privateCommandHandlers.TryRemove(key, out CommandHandler v))
            {
                await Task.Delay(1000);
                await DisposePrivateCommandHandlerAsync(key);
            }
        }

		public async Task<IMessage> ListenNextMessageAsync(ulong channelId, ulong userId)
		{
			IMessage outputMessage = null;

			if (nextMessageRequests.TryAdd(new Tuple<ulong, ulong>(userId, channelId), (msg) =>
			 {
				 outputMessage = msg;
				 nextMessageRequests.TryRemove(new Tuple<ulong, ulong>(userId, channelId), out Action<IMessage> v);
			 }))
			{
				while (outputMessage == null)
				{
					await Task.Delay(100);
				}
			}
			return outputMessage;
		}

        internal async Task DisposePrivateCommandHandlerAsync(IMessage msg)
        {
            await DisposePrivateCommandHandlerAsync(new Tuple<ulong, ulong>(msg.Author.Id, msg.Channel.Id));
        }

        public Event GetEventByName(string id)
        {
            return events.GetEvent(id);
        }

        public async Task<SortedDictionary<string, List<string>>> GetEventNamesAsync(IMessage e)
        {
            SortedDictionary<string, List<string>> moduleEvents = new SortedDictionary<string, List<string>>
            {
                { "MISC", new List<string>() }
            };

            EventAccessibility userEventAccessibility = CommandHandler.GetUserAccessibility(e);

            foreach (CommandEvent ev in CommandHandler.Commands)
            {
                if (await ev.IsEnabled(e.Channel.Id) && userEventAccessibility >= ev.Accessibility)
                {
                    if (ev.Module != null)
                    {
                        if (!moduleEvents.ContainsKey(ev.Module.Name.ToUpper()))
                        {
                            moduleEvents.Add(ev.Module.Name.ToUpper(), new List<string>());
                        }

                        if (CommandHandler.GetUserAccessibility(e) >= ev.Accessibility)
                        {
                            moduleEvents[ev.Module.Name.ToUpper()].Add(ev.Name);
                        }
                    }
                    else
                    {
                        moduleEvents["MISC"].Add(ev.Name);
                    }
                }
            }

            if (moduleEvents["MISC"].Count == 0)
            {
                moduleEvents.Remove("MISC");
            }

            moduleEvents.OrderBy(i => { return i.Key; });

            foreach (List<string> list in moduleEvents.Values)
            {
                list.Sort((x, y) => x.CompareTo(y));
            }

            return moduleEvents;
        }

        public async Task<string> GetIdentifierAsync(ulong guildId, PrefixInstance prefix)
        {
            using (var context = new IAContext())
            {
                Identifier i = await context.Identifiers.FindAsync(guildId);
                if (i == null)
                {
                    i = context.Identifiers.Add(new Identifier()
					{
						GuildId = guildId.ToDbLong(),
						Value = prefix.DefaultValue
					}).Entity;
                    await context.SaveChangesAsync();
                }
                return i.Value;
            }       
        }

        public PrefixInstance GetPrefixInstance(string defaultPrefix)
        {
            string prefix = defaultPrefix.ToLower();

            if (commandHandler.Prefixes.ContainsKey(prefix))
            {
                return commandHandler.Prefixes[prefix];
            }
            return null;
        }

        public Module GetModuleByName(string name)
        {
			Module m = CommandHandler.GetModule(name.ToLower());

			if (m != null)
            {
				return m;
            }
            Log.Warning($"Could not find Module with name '{name}'");
            return null;
        }

        public async Task<string> ListCommandsAsync(IMessage e)
        {
            SortedDictionary<string, List<string>> moduleEvents = await GetEventNamesAsync(e);

            string output = "";
            foreach (KeyValuePair<string, List<string>> items in moduleEvents)
            {
                output += "**" + items.Key + "**\n";
                for (int i = 0; i < items.Value.Count; i++)
                {
                    output += items.Value[i] + ", ";
                }
                output = output.Remove(output.Length - 2);
                output += "\n\n";
            }
            return output;
        }

        public async Task<Embed> ListCommandsInEmbedAsync(IMessage e)
        {
            SortedDictionary<string, List<string>> moduleEvents = await GetEventNamesAsync(e);

			EmbedBuilder embed = new EmbedBuilder();

            foreach (KeyValuePair<string, List<string>> items in moduleEvents)
            {
                for(int i = 0; i < items.Value.Count; i ++)
                {
                    items.Value[i] = $"`{items.Value[i]}`";
                }        

                embed.AddField(items.Key, string.Join(", ",items.Value));
            }
            return embed.Build();
        }

        public void RegisterAttributeCommands()
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            var modules = assembly.GetTypes()
                                  .Where(m => m.GetCustomAttributes<ModuleAttribute>().Count() > 0)
                                  .ToArray();

            foreach (var m in modules)
            {
                Module newModule = new Module();
                object instance = null;

                try
                {
                    instance = Activator.CreateInstance(Type.GetType(m.AssemblyQualifiedName), newModule);
                }
                catch
                {
                    instance = Activator.CreateInstance(Type.GetType(m.AssemblyQualifiedName));
                }

                newModule.EventSystem = this;

                ModuleAttribute mAttrib = m.GetCustomAttribute<ModuleAttribute>();
                newModule.Name = mAttrib.module.Name.ToLower();
                newModule.Nsfw = mAttrib.module.Nsfw;
                newModule.CanBeDisabled = mAttrib.module.CanBeDisabled;

                var methods = m.GetMethods()
                               .Where(t => t.GetCustomAttributes<CommandAttribute>().Count() > 0)
                               .ToArray();

                foreach (var x in methods)
                {
                    CommandEvent newEvent = new CommandEvent();
                    CommandAttribute commandAttribute = x.GetCustomAttribute<CommandAttribute>();

                    newEvent = commandAttribute.command;
                    newEvent.ProcessCommand = async (context) => await (Task)x.Invoke(instance, new object[] { context });
                    newEvent.Module = newModule;

                    CommandEvent foundCommand = newModule.Events.Find(c => c.Name == newEvent.Name);

                    if (foundCommand != null)
                    {
                        if (commandAttribute.on != "")
                        {
                            foundCommand.On(commandAttribute.On, newEvent.ProcessCommand);
                        }
                        else
                        {
                            foundCommand.Default(newEvent.ProcessCommand);
                        }
                    }
                    else
                    {
                        newModule.AddCommand(newEvent);
                    }
                }

				newModule.Install(bot);
            }
        }

        public static EventSystem Start(Bot bot)
        {
            _instance = new EventSystem(bot);
			_instance.RegisterAttributeCommands();
			return _instance;
		}

		public PrefixInstance RegisterPrefixInstance(string prefix, bool canBeChanged = true, bool forceExecuteCommands = false)
        {
            PrefixInstance newPrefix = new PrefixInstance(prefix.ToLower(), canBeChanged, forceExecuteCommands);
            commandHandler.Prefixes.Add(prefix, newPrefix);
            return newPrefix;
        }

        #region events

        internal async Task OnCommandDone(IMessage e, CommandEvent commandEvent, bool success = true, float time = 0.0f)
        {
            foreach (CommandDoneEvent ev in events.CommandDoneEvents.Values)
            {
				Stopwatch sw = Stopwatch.StartNew();

				try
				{
                    await ev.processEvent(e, commandEvent, success, time);
                }
                catch (Exception ex)
                {
					Log.Error(ex);
				}
			}
        }

        private async Task OnPrivateMessage(IMessage arg)
        {
            await Task.CompletedTask;
        }

        private async Task OnMessageRecieved(IMessage msg)
        {
            if (msg.Author.IsBot || ignore.Contains(msg.Author.Id))
            {
                return;
            }

            await commandHandler.CheckAsync(msg);

            foreach (CommandHandler c in commandHandlers)
            {
				if (c.ShouldBeDisposed && c.ShouldDispose())
				{
					commandHandlers.Remove(c);
				}

                await c.CheckAsync(msg);
            }

            Tuple<ulong, ulong> privateKey = new Tuple<ulong, ulong>(msg.Author.Id, msg.Channel.Id);

            if (privateCommandHandlers.ContainsKey(privateKey))
            {
                if (privateCommandHandlers[privateKey].ShouldBeDisposed && privateCommandHandlers[privateKey].ShouldDispose())
                {
                    await DisposePrivateCommandHandlerAsync(msg);
                }
                else
                {
                    await privateCommandHandlers[privateKey].CheckAsync(msg);
                }
            }

			if (nextMessageRequests.TryGetValue(privateKey, out Action<IMessage> action))
			{
				action(msg);
			}
        }

        private void AddPrivateCommandHandler(Tuple<ulong, ulong> key, CommandHandler value)
        {
            privateCommandHandlers.AddOrUpdate(key, value,
                (k, existingVal) =>
                {
                    if (value != existingVal)
                    {
                        return existingVal;
                    }
                    return value;
                });
        }

        public void AddPrivateCommandHandler(IMessage msg, CommandHandler cHandler)
        {
            AddPrivateCommandHandler(new Tuple<ulong, ulong>(msg.Author.Id, msg.Channel.Id), cHandler as CommandHandler);
        }

		private async Task InternalMessageReceived(SocketMessage message)
		{
			if (message.Channel is IGuildChannel)
			{
				try
				{
					Task.Run(async () => await OnMessageRecieved(message));
				}
				catch (Exception e)
				{
					Log.Error(e);
				};
			}
			else
			{
				Log.Message($"[DM from {message.Author.ToString()}] {message.Content}");
			}
		}

        #endregion events
    }

    public delegate void OnRegisteredMessage(IMessage m);
}