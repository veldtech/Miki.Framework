using System.Threading.Tasks;

namespace Miki.Framework.Commands.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IFilter
    {
        ValueTask<bool> CheckAsync(IContext e);
    }
}
