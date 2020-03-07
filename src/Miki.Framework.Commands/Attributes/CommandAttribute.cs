namespace Miki.Framework.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
	/// Command attributes are notated to show that this asynchronous method is a command that the
	/// bot can execute. Keep in mind if you use the default <see cref="CommandTree"/> this will require
	/// you to encapsulate your current method into a class marked with a <see cref="ModuleAttribute"/>
	/// attribute to be able to be found. Command attributes can be assigned to both methods and types.
	/// However, both targets require different semantics:
	/// - For a method, you need to have an <see cref="IContext"/> as your first argument, and it needs to
	/// return a <see cref="System.Threading.Tasks.Task"/>.
	/// - For a type, you need to either enrich it with additional tasks that are commands. Child-commands
	/// can be empty, but this will be used as the default execution line for the type command. An empty
	/// child command needs to be unique. This execution is recursive-friendly.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
	public class CommandAttribute : Attribute
	{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'CommandAttribute.Aliases'
		public IReadOnlyCollection<string> Aliases { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'CommandAttribute.Aliases'

		/// <summary>
		/// Used as a default execution for child commands. Should not be used inside of modules.
		/// <seealso cref="CommandAttribute"/> for more information.
		/// </summary>
		public CommandAttribute() { }

		/// <summary>
		/// Creates a default command. Aliases need to be unique all across the current scope. The
		/// first alias will be used as its unique command ID.
		/// <seealso cref="CommandAttribute"/> for more information.
        /// </summary>
		/// <param name="aliases">All aliases that the command should react to.</param>
		public CommandAttribute(params string[] aliases)
		{
			if(aliases.Any(string.IsNullOrWhiteSpace))
			{
				throw new ArgumentNullException(nameof(aliases), "Alias cannot be empty or null.");
			}
			Aliases = aliases;
		}

		internal CommandMetadata AsMetadata()
		{
			return new CommandMetadata
			{
				Identifiers = Aliases
			};
		}
	}
}