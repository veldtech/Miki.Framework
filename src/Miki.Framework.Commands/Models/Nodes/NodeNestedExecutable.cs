﻿using System.Collections.Generic;

namespace Miki.Framework.Commands.Nodes
{
    using Miki.Framework.Arguments;
    using Miki.Logging;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
	/// Executable Node that can contain multiple nodes, while keeping a default executable.
	/// </summary>
	public class NodeNestedExecutable : NodeContainer, IExecutable
	{
		private CommandDelegate runAsync;

        /// <summary>
		/// Creates a new Nested, Executable Node.
		/// </summary>
		/// <param name="metadata">Command properties.</param>
		/// <param name="builder"></param>
		/// <param name="t"></param>
        public NodeNestedExecutable(
			CommandMetadata metadata,
			IServiceProvider builder,
			Type t)
			: this(metadata, (NodeContainer) null, t)
		{
		}

        /// <summary>
        /// Creates a new Nested, Executable Node.
        /// </summary>
        /// <param name="metadata">Command properties.</param>
        /// <param name="parent"></param>
        /// <param name="t"></param>
        public NodeNestedExecutable(
			CommandMetadata metadata,
			NodeContainer parent,
			Type t)
			: base(metadata, parent, t)
		{
		}

		/// <summary>
		/// Sets the fallback executable.
		/// </summary>
		/// <param name="defaultTask"></param>
		public void SetDefaultExecution(CommandDelegate defaultTask)
		{
			runAsync = defaultTask;
		}

		/// <summary>
		/// Query a command recursively
		/// </summary>
		/// <param name="pack">Argument pack to iterate over.</param>
		/// <returns>Nullable node</returns>
		public override IEnumerable<NodeResult> FindCommands(IArgumentPack pack)
		{
			var arg = pack.Peek().Unwrap();

            if (Metadata.Identifiers == null)
            {
	            yield break;
            }

            var count = Metadata.Identifiers.Count;

            if (count == 0)
            {
	            yield break;
            }

            if (count == 1)
            {
	            if (!string.Equals(Metadata.Identifiers[0], arg, StringComparison.OrdinalIgnoreCase))
	            {
		            yield break;
	            }
            }
            else if (Metadata.Identifiers.All(x => !string.Equals(x, arg, StringComparison.OrdinalIgnoreCase)))
            {
	            yield break;
            }
            
            pack.SetCursor(pack.Cursor + 1);

            foreach (var command in base.FindCommands(pack))
            {
	            yield return command;
            }

            yield return new NodeResult(this, pack.Cursor);
            
            pack.SetCursor(pack.Cursor - 1);
		}

		/// <summary>
		/// Executes command.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async ValueTask ExecuteAsync(IContext context)
		{
			foreach(var v in Attributes.OfType<ICommandRequirement>())
			{
                if(await v.CheckAsync(context))
                {
                    continue;
                }
                await v.OnCheckFail(context);
                return;
            }

			if(runAsync == null)
			{
				Log.Warning("Default executable not found; omitting request.");
				return;
			}

			await runAsync(context);
		}
	}
}
