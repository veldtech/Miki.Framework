using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Miki.Framework.Events.Commands
{
	public class MessageListener : CommandHandler
	{
		ConcurrentDictionary<CommandSession, Action<IMessage>> sessionCache = new ConcurrentDictionary<CommandSession, Action<IMessage>>();

		public async Task<IMessage> WaitForNextMessage(ulong userId, ulong channelId, int timeOutInMilliseconds = 10000)
			=> await WaitForNextMessage(new CommandSession { ChannelId = channelId, UserId = userId }, timeOutInMilliseconds);
		public async Task<IMessage> WaitForNextMessage(CommandSession session, int timeOutInMilliseconds = 10000)
		{
			IMessage nextMessage = null;

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

		public override async Task CheckAsync(MessageContext context)
		{
			CommandSession session;
			session.ChannelId = context.message.Channel.Id;
			session.UserId = context.message.Author.Id;

			if (sessionCache.ContainsKey(session))
			{
				if (sessionCache.TryGetValue(session, out Action<IMessage> a))
				{
					a(context.message);
				}
			}
		}
	}
}
