using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework
{
    public class ProviderCollection : IEnumerable<IProvider>, IProvider
    {
        /// <inheritdoc/>
        public bool IsActive { get; private set; }

        private readonly List<IProvider> providers = new List<IProvider>();

        public ProviderCollection Add(IProvider provider)
        {
            if (provider == this)
            {
                throw new InvalidOperationException("Cannot insert self in collection");
            }

            if (providers.Contains(provider))
            {
                throw new InvalidOperationException("Cannot insert duplicate references");
            }

            providers.Add(provider);
            return this;
        }

        public Task StartAsync()
        {
            IsActive = true;
            return Task.WhenAll(this.Select(x => x.StartAsync()));
        }

        public Task StopAsync()
        {
            IsActive = false;
            return Task.WhenAll(this.Select(x => x.StopAsync()));
        }

        public IEnumerator<IProvider> GetEnumerator()
        {
            return providers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}