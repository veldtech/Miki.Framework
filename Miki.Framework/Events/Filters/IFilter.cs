using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Filters
{
    public interface IFilter
    {
		Task<bool> FilterAsync(IMessage msg);
    }
}
