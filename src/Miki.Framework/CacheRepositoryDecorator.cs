namespace Miki.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Cache;
    using Patterns.Repositories;

    /// <summary>
    /// Cached decorator pattern to automatically cache a fetched repository entity.
    /// </summary>
    /// <typeparam name="T">Entity Type</typeparam>
    public class CachedRepository<T> : IAsyncRepository<T>
        where T : class
    {
        private readonly IAsyncRepository<T> innerRepository;
        private readonly ICacheClient cache;
        private readonly Config config;

        /// <summary>
        /// Cached decorator pattern to automatically cache a fetched repository entity.
        /// </summary>
        /// <param name="innerRepository">Actual data source.</param>
        /// <param name="cache">Cache client instance.</param>
        /// <param name="config">Configuration for this decorator.</param>
        public CachedRepository(
            IAsyncRepository<T> innerRepository,
            ICacheClient cache,
            Config config)
        {
            this.innerRepository = innerRepository;
            this.cache = cache;
            this.config = config;
            if (config.KeySelector == null)
            {
                throw new ArgumentNullException(
                    $"{nameof(config)}.{nameof(config.KeySelector)} was null.");
            }
        }

        /// <inheritdoc />
        public async ValueTask<T> GetAsync(params object[] id)
        {
            var contextKey = CreateKey(id);
            var obj = await cache.GetAsync<T>(contextKey);
            if (obj != default)
            {
                return obj;
            }

            var baseObject = await innerRepository.GetAsync(id);
            if (baseObject != default)
            {
                await cache.UpsertAsync(contextKey, baseObject, config.Lifetime);
            }

            return baseObject;
        }

        /// <inheritdoc />
        public ValueTask<IEnumerable<T>> ListAsync()
        {
            return innerRepository.ListAsync();
        }

        /// <inheritdoc />
        public ValueTask<T> AddAsync(T entity)
        {
            return innerRepository.AddAsync(entity);
        }

        /// <inheritdoc />
        public async ValueTask EditAsync(T entity)
        {
            await cache.UpsertAsync(CreateKey(entity), entity, config.Lifetime);
            await innerRepository.EditAsync(entity);
        }

        /// <inheritdoc />
        public async ValueTask DeleteAsync(T entity)
        {
            await cache.RemoveAsync(CreateKey(entity));
            await innerRepository.DeleteAsync(entity);
        }

        private string CreateKey(T p)
        {
            return CreateKey(config.KeySelector.Invoke(p));
        }

        private string CreateKey(object[] p)
        {
            return config.Namespace + string.Join(":", p);
        }

        public class Config
        {
            /// <summary>
            /// Should align with your "get" parameters.
            /// </summary>
            [NotNull]
            public Func<T, object[]> KeySelector { get; set; }

            /// <summary>
            /// Overrides the keyspace root
            /// </summary>
            public string Namespace { get; set; } = typeof(T).Name.ToLowerInvariant();
            
            /// <summary>
            /// Time-To-Live for cache objects.
            /// </summary>
            public TimeSpan? Lifetime { get; set; }
        }
    }
}
