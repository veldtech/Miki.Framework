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
using System.Threading;
using Miki.Framework.Events.Filters;
using Miki.Framework.Events.Commands;

namespace Miki.Framework.Events
{
	public class EventSystem : IAttachable
	{
		public event Func<Exception, CommandEvent, IMessage, Task> OnCommandDone;

		private Bot bot;
		private EventSystemConfig config;

		public MessageFilter MessageFilter { get; private set; } = new MessageFilter();

		public List<ICommandHandler> commandHandlers = new List<ICommandHandler>();

		public EventSystem(EventSystemConfig config)
		{
			this.config = config;
		}

		public void AttachTo(Bot bot)
		{
			this.bot = bot;
			bot.Client.MessageReceived += OnMessageReceivedAsync;
		}

		public async Task OnMessageReceivedAsync(IMessage msg)
		{
			if (!await MessageFilter.Run(msg))
			{
				return;
			}
			
			foreach(var handler in commandHandlers)
			{
				await handler.CheckAsync(msg);
			}
		}
	}
}