namespace Miki.Framework.Commands.Prefixes
{
    using System.Threading.Tasks;
    using Miki.Framework.Commands.Prefixes.Triggers;

    public interface IPrefixService
    {
        ITrigger GetDefaultTrigger();

        ValueTask<string> MatchAsync(IContext context);
    }
}