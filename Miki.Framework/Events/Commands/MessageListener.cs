using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Logging;

namespace Miki.Framework.Events.Commands
{
	public class MessageListener : CommandHandler
	{
		ConcurrentDictionary<CommandSession, Action<IDiscordMessage>> sessionCache = new ConcurrentDictionary<CommandSession, Action<IDiscordMessage>>();

		public async Task<IDiscordMessage> WaitForNextMessage(ulong userId, ulong channelId, int timeOutInMilliseconds = 10000)
			=> await WaitForNextMessage(new CommandSession { ChannelId = channelId, UserId = userId }, timeOutInMilliseconds);
		public async Task<IDiscordMessage> WaitForNextMessage(CommandSession session, int timeOutInMilliseconds = 10000)
		{
			IDiscordMessage nextMessage = null;

			if (sessionCache.TryAdd(session, (msg) =>
			 {
				 nextMessage = msg;
			 }))
			{
				while (nextMessage == null)
				{
					await Task.Delay(100);
					timeOutInMilliseconds -= 100;

					if(timeOutInMilliseconds <= 0)
					{
						throw new TimeoutException();
					}
				}
				sessionCache.TryRemove(session, out var x);
			}

			return nextMessage;
		}	

		public override async Task CheckAsync(EventContext context)
		{
			await Task.Yield();
			CommandSession session;
			session.ChannelId = context.message.ChannelId;
			session.UserId = context.message.Author.Id;

			if (sessionCache.ContainsKey(session))
			{
				if (sessionCache.TryGetValue(session, out Action<IDiscordMessage> a))
				{
					a(context.message);
				}
			}
		}
	}
}
