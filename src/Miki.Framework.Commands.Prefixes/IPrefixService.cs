using System.Threading.Tasks;
using Miki.Framework.Commands.Prefixes.Triggers;

namespace Miki.Framework.Commands.Prefixes
{
    public interface IPrefixService
    {
        ITrigger GetDefaultTrigger();

        ValueTask<string> MatchAsync(IContext context);
    }
}