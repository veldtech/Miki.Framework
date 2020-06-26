﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Framework.Hosting
{
    public delegate ValueTask MessageDelegate(IContext context);

    public interface IBotApplicationBuilder
    {
        /// <summary>
        /// Gets or sets the <see cref="IServiceProvider"/> that provides access to the application's service container.
        /// </summary>
        IServiceProvider ApplicationServices { get; set; }

        /// <summary>
        /// Gets a key/value collection that can be used to share data between middleware.
        /// </summary>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Get the property by its key.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key of the property.</param>
        /// <returns>The value of the property.</returns>
        T GetProperty<T>(string key);

        /// <summary>
        /// Set the property by its key.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key of the property.</param>
        /// <param name="value">The new value.</param>
        void SetProperty<T>(string key, T value);

        /// <summary>
        /// Adds a middleware delegate to the application's request pipeline.
        /// </summary>
        /// <param name="middleware">The delegate middleware.</param>
        /// <returns>The current application builder.</returns>
        IBotApplicationBuilder Use(Func<MessageDelegate, MessageDelegate> middleware);

        /// <summary>
        /// Builds the delegate used by this application to process Discord messages.
        /// </summary>
        /// <returns></returns>
        MessageDelegate Build();

        /// <summary>
        /// Create a sub-builder.
        /// </summary>
        IBotApplicationBuilder New();
    }
}