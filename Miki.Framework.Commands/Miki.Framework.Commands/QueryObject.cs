using Miki.Discord.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Commands
{
    public class QueryObject
    {
		private Queue<string> _query;

		public string Current => _query.Peek();

		public QueryObject(IDiscordMessage message)
		{
			_query = new Queue<string>(message.Content.Split(' '));
		}

		public void Next()
		{
			if(_query.Count > 0)
			{
				_query.Dequeue();
			}
		}

		public override string ToString()
		{
			return string.Join(" ", _query.ToArray());
		}
	}
}
