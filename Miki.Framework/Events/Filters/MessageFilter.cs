using Miki.Discord.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Filters
{
    public class MessageFilter
    {
		private List<IFilter> filters = new List<IFilter>();

		public void AddFilter(IFilter filter)
		{
			filters.Add(filter);
		}

		public async Task<bool> IsAllowedAsync(IDiscordMessage msg)
		{
			foreach(IFilter f in filters)
			{
				if(await f.FilterAsync(msg))
				{
					return false;
				}
			}
			return true;
		}

		public T Get<T>() where T : IFilter
		{
			foreach(var f in filters)
			{
				if(f is T t)
				{
					return t;
				}
			}
			return default(T);
		}
	}
}
