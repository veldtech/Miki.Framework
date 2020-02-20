using System.Linq;
using Miki.Framework.Commands.Prefixes.Triggers;

namespace Miki.Framework.Commands.Prefixes
{
    using System.Threading.Tasks;

    public class PrefixService : IPrefixService
    {
        private readonly PrefixCollection collection;

        public PrefixService(PrefixCollection collection)
        {
            this.collection = collection;
        }

        public ITrigger GetDefaultTrigger()
        {
            return collection.DefaultTrigger;
        }

        public async ValueTask<string> MatchAsync(IContext context)
        {
            foreach (var x in collection)
            {
                string v = await x.CheckTriggerAsync(context);
                if (v != null)
                {
                    return v;
                }
            }
            return null;
        }
    }
}
