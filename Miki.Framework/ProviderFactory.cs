using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework
{
    public sealed class ProviderAdapter : IProvider
    {
        private readonly Func<Task> startAsync;
        private readonly Func<Task> stopAsync;

        private ProviderAdapter(
            Func<Task> start,
            Func<Task> stop)
        {
            this.startAsync = start;
            this.stopAsync = stop;
        }
        public static ProviderAdapter Factory(
            Func<Task> startAsync,
            Func<Task> stopAsync)
        {
            return new ProviderAdapter(startAsync, stopAsync);
        }

        public bool IsActive { get; private set; }
        public Task StartAsync()
        {
            IsActive = true;
            return startAsync();
        }

        public Task StopAsync()
        {
            IsActive = true;
            return stopAsync();
        }
    }
}
