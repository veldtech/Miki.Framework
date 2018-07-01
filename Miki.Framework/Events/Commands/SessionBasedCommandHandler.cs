using Miki.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Commands
{
	public class SessionBasedCommandHandler : CommandHandler
	{
		public Dictionary<CommandSession, Tuple<CommandHandler, DateTime>> Sessions { get; private set; } = new Dictionary<CommandSession, Tuple<CommandHandler, DateTime>>();

		public void AddSession(CommandSession session, CommandHandler handler, TimeSpan? expiration = null)
		{
			if(Sessions.TryGetValue(session, out var handlerTuple))
			{
				if(handlerTuple.Item2 >= DateTime.Now)
				{
					throw new SessionInUseException();
				}

				Sessions.Remove(session);
			}

			Sessions.Add(session, new Tuple<CommandHandler, DateTime>(handler, DateTime.Now + (expiration ?? TimeSpan.FromSeconds(30))));
		}

		public override async Task CheckAsync(MessageContext context)
		{
			CommandSession session;

			session.ChannelId = context.message.ChannelId;
			session.UserId = context.message.Author.Id;

			if(Sessions.TryGetValue(session, out var commandHandler))
			{
				if(commandHandler.Item2 < DateTime.Now)
				{
					Sessions.Remove(session);
				}

				await commandHandler.Item1.CheckAsync(context);
			}
		}

		public void RemoveSession(ulong userId, ulong channelId)
		{
			CommandSession s;
			s.ChannelId = channelId;
			s.UserId = userId;
			RemoveSession(s);
		}
		public void RemoveSession(CommandSession session)
		{
			Sessions.Remove(session);
		}
	}
}
