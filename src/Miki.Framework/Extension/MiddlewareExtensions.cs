﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Miki.Framework.Hosting;

namespace Miki.Framework
{
    using Predicate = Func<IContext, bool>;
    using PredicateAsync = Func<IContext, ValueTask<bool>>;
    
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Adds a middleware delegate defined in-line to the application's request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IBotApplicationBuilder"/> instance.</param>
        /// <param name="middleware">A function that handles the request or calls the given next function.</param>
        /// <returns>The <see cref="IBotApplicationBuilder"/> instance.</returns>
        public static IBotApplicationBuilder Use(this IBotApplicationBuilder app, Func<IContext, Func<ValueTask>, ValueTask> middleware)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            
            if (middleware == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            
            return app.Use(next =>
            {
                return context =>
                {
                    return middleware(context, () => next(context));
                };
            });
        }
        
        /// <summary>
        /// Adds a terminal middleware delegate to the application's request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IBotApplicationBuilder"/> instance.</param>
        /// <param name="handler">A delegate that handles the request.</param>
        public static void Run(this IBotApplicationBuilder app, MessageDelegate handler)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            app.Use(_ => handler);
        }
        
        /// <summary>
        /// Conditionally creates a branch in the request pipeline that is rejoined to the main pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="predicate">Invoked with the request environment to determine if the branch should be taken</param>
        /// <param name="configuration">Configures a branch to take</param>
        /// <returns></returns>
        public static IBotApplicationBuilder UseWhen(this IBotApplicationBuilder app, Predicate predicate, Action<IBotApplicationBuilder> configuration)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            
            // Create and configure the branch builder right away; otherwise,
            // we would end up running our branch after all the components
            // that were subsequently added to the main builder.
            var branchBuilder = app.New();
            configuration(branchBuilder);

            return app.Use(main =>
            {
                // This is called only when the main application builder 
                // is built, not per request.
                branchBuilder.Run(main);
                var branch = branchBuilder.Build();

                return context => predicate(context) ? branch(context) : main(context);
            });
        }
        
        /// <summary>
        /// Conditionally creates a branch in the request pipeline that is rejoined to the main pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="predicate">Invoked with the request environment to determine if the branch should be taken</param>
        /// <param name="configuration">Configures a branch to take</param>
        /// <returns></returns>
        public static IBotApplicationBuilder UseWhen(this IBotApplicationBuilder app, PredicateAsync predicate, Action<IBotApplicationBuilder> configuration)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            
            // Create and configure the branch builder right away; otherwise,
            // we would end up running our branch after all the components
            // that were subsequently added to the main builder.
            var branchBuilder = app.New();
            configuration(branchBuilder);

            return app.Use(main =>
            {
                // This is called only when the main application builder 
                // is built, not per request.
                branchBuilder.Run(main);
                var branch = branchBuilder.Build();

                return async context =>
                {
                    if (await predicate(context))
                    {
                        await branch(context);
                    }
                    else
                    {
                        await main(context);
                    }
                };
            });
        }
    }
}