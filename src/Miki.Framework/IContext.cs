namespace Miki.Framework
{
    using System;

    /// <summary>
    /// Session context for a single command. Keeps data and services for this specific session.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// The command executed in this current session.
        /// </summary>
        IExecutable Executable { get; }

        /// <summary>
        /// Services built in <see cref="MikiApp"/>
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// Context objects are used for specific session-only objects that are added through pipeline
        /// objects.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        object GetContext(string id);

        /// <summary>
        /// Used to retrieve services built in <see cref="MikiApp"/>
        /// </summary>
        object GetService(Type t);
    }
}