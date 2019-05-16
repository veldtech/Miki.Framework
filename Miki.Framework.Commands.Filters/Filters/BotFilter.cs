using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Filters.Filters
{
    /// <summary>
    /// Filters bot accounts
    /// </summary>
    public class BotFilter : IFilter
    {
        public ValueTask<bool> CheckAsync(IContext e)
        {
            return new ValueTask<bool>(!e.GetMessage().Author.IsBot);
        }
    }
}
