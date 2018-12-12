using Miki.Cache;
using Miki.Framework.Exceptions;
using Miki.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Commands
{
	public class SessionBasedCommandHandler : CommandHandler
	{
		public ConcurrentDictionary<CommandSession, Tuple<CommandHandler, DateTime>> Sessions { get; private set; } = new ConcurrentDictionary<CommandSession, Tuple<CommandHandler, DateTime>>();

		public SessionBasedCommandHandler(ICacheClient cachePool)
			: base(cachePool)
		{
		}

		public async Task AddSessionAsync(CommandSession session, CommandHandler handler, TimeSpan? expiration = null)
		{
			if (Sessions.TryGetValue(session, out var handlerTuple))
			{
				if (handlerTuple.Item2 >= DateTime.Now)
				{
					throw new SessionInUseException();
				}

				await RemoveSessionAsync(session);
			}

			while (!Sessions.TryAdd(session, new Tuple<CommandHandler, DateTime>(handler, DateTime.Now + (expiration ?? TimeSpan.FromSeconds(30)))))
			{
				await Task.Delay(100);
			}
		}

		public override async Task CheckAsync(EventContext context)
		{
			try
			{
				CommandSession session;

				session.ChannelId = context.message.ChannelId;
				session.UserId = context.message.Author.Id;

				if (Sessions.TryGetValue(session, out var commandHandler))
				{
					if (commandHandler.Item2 < DateTime.Now)
					{
						await RemoveSessionAsync(session);
					}

					await commandHandler.Item1.CheckAsync(context);
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		public async Task RemoveSessionAsync(ulong userId, ulong channelId)
		{
			CommandSession s;
			s.ChannelId = channelId;
			s.UserId = userId;
			await RemoveSessionAsync(s);
		}

		public async Task RemoveSessionAsync(CommandSession session)
		{
			while (!Sessions.TryRemove(session, out var x))
			{
				await Task.Delay(100);
			}
		}
	}
}