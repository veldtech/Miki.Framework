namespace Miki.Framework.Commands.Filters
{
    using System.Threading.Tasks;

    public interface IFilter
    {
        ValueTask<bool> CheckAsync(IContext e);
    }
}
