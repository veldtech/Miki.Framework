namespace Miki.Framework.Commands.Scopes
{
    using System;
    using System.Threading.Tasks;
    using Models;
    using Patterns.Repositories;

    public class ScopeService
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

            await repository.AddAsync(scope)
                .ConfigureAwait(false);
            await context.CommitAsync()
                .ConfigureAwait(false);
        }

        public async ValueTask<bool> HasScopeAsync(long userId, string scopeName)
        {
            if (string.IsNullOrWhiteSpace(scopeName))
            {
                throw new ArgumentNullException(nameof(scopeName));
            }

            var scope = await repository.GetAsync(userId, scopeName)
                .ConfigureAwait(false);
            return scope != null;
        }
    }
}
