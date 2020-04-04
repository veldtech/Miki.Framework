namespace Miki.Framework.Commands.Filters
{
    using System.Threading.Tasks;

    /// <summary>
    /// Asynchronous filter interface for the <see cref="FilterPipelineStage"/>.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Check whether the current context passes the filter.
        /// </summary>
        ValueTask<bool> CheckAsync(IContext e);
    }
}
