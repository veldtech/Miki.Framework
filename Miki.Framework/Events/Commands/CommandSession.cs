using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Events.Commands
{
	public struct CommandSession
	{
		public ulong UserId;
		public ulong ChannelId;

		public override bool Equals(object obj)
		{
			if (obj is CommandSession session)
			{
				return session == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(UserId, ChannelId);
		}

		public static bool operator ==(CommandSession c1, CommandSession c2)
		{
			return c1.ChannelId == c2.ChannelId && c1.UserId == c2.UserId;
		}
		public static bool operator !=(CommandSession c1, CommandSession c2)
		{
			return c1.ChannelId != c2.ChannelId || c1.UserId != c2.UserId;
		}
	}
}
