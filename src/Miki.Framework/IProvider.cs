namespace Miki.Framework
{
    using System.Threading.Tasks;

    /// <summary>
    /// Can be used to have services with an active connection turn on and off.
    /// </summary>
    public interface IProvider
    {
        // TODO: add an reactive stream as default fetcher?

        /// <summary>
        /// Whether the current service has its internal threads collecting for
        /// additional events.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Enabled the aggregation of events.
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Disables the aggregation of events.
        /// </summary>
        Task StopAsync();
    }
}
