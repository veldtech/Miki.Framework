using System.Linq;
using Miki.Framework.Commands.Prefixes.Triggers;

namespace Miki.Framework.Commands.Prefixes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Miki.Framework.Events.Triggers;

    public class PrefixCollection<T> : List<ITrigger<T>>
    { }

    public class PrefixService<T>
    {
        private readonly PrefixCollection<T> collection;

        public PrefixService(PrefixCollection<T> collection)
        {
            this.collection = collection;
        }

        public PrefixTrigger GetDefaultTrigger()
        {
            return collection.OfType<PrefixTrigger>()
                .FirstOrDefault(x => x.IsDefault);
        }

        public async ValueTask<string> MatchAsync(IContext context, T value)
        {
            foreach (var x in collection)
            {
                string v = await x.CheckTriggerAsync(context, value);
                if (v != null)
                {
                    return v;
                }
            }
            return null;
        }
    }
}
