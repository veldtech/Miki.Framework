using System;
using System.Threading.Tasks;

namespace Miki.Framework
{
    public sealed class ProviderAdapter : IProvider
    {
        private readonly Func<Task> startAsync;
        private readonly Func<Task> stopAsync;

        public ProviderAdapter(
            Func<Task> start,
            Func<Task> stop)
        {
            this.startAsync = start;
            this.stopAsync = stop;
        }

        [Obsolete("Use the default constructor instead.")]
        public static ProviderAdapter Factory(
            Func<Task> startAsync,
            Func<Task> stopAsync)
        {
            return new ProviderAdapter(startAsync, stopAsync);
        }

        /// <inheritdoc/>
        public bool IsActive { get; private set; }

        /// <inheritdoc/>
        public Task StartAsync()
        {
            IsActive = true;
            return startAsync();
        }

        /// <inheritdoc/>
        public Task StopAsync()
        {
            IsActive = true;
            return stopAsync();
        }
    }
}
