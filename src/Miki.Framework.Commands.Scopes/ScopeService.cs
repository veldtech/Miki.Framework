namespace Miki.Framework.Commands.Scopes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Patterns.Repositories;

    public interface IScopeService
    {
        /// <summary>
        /// Adds a new scope to a user.
        /// </summary>
        ValueTask AddScopeAsync(Scope scope);
        
        /// <summary>
        /// Check if the user with Id <paramref name="userId"/> has all scopes listed in <paramref name="scopeNames"/>.
        /// </summary>
        /// <param name="userId">User that is trying to access this scoped operation.</param>
        /// <param name="scopeNames">List of scope IDs that the user needs to have.</param>
        ValueTask<bool> HasScopeAsync(long userId, IEnumerable<string> scopeNames);
    }

    public class ScopeService : IScopeService
    {
        private readonly IUnitOfWork context;
        private readonly IAsyncRepository<Scope> repository; 

        public ScopeService(IUnitOfWork context)
        {
            this.context = context;
            this.repository = context.GetRepository<Scope>();
        }

        public async ValueTask AddScopeAsync(Scope scope)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            await repository.AddAsync(scope).ConfigureAwait(false);
            await context.CommitAsync().ConfigureAwait(false);
        }

        public async ValueTask<bool> HasScopeAsync(long userId, IEnumerable<string> scopeNames)
        {
            if(scopeNames == null)
            {
                return true;
            }

            var validNames = scopeNames.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            if(!validNames.Any())
            {
                return true;
            }

            var scopeQuery = (await repository.ListAsync())
                .Where(x => x.UserId == userId && validNames.Contains(x.ScopeId));
            if(!(scopeQuery is IQueryable<Scope> queryable))
            {
                return scopeQuery.ToList().Count == validNames.Count;
            }

            var scopes = await queryable.ToListAsync().ConfigureAwait(false);
            return scopes.Count == validNames.Count;
        }
    }
}
