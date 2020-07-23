using Miki.Framework.Arguments;
using Miki.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Nodes
{
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
			: this(metadata, null, builder, t)
		{
		}

		/// <summary>
		/// Creates a new Nested, Executable Node.
		/// </summary>
		/// <param name="metadata">Command properties.</param>
		/// <param name="parent"></param>
		/// <param name="builder"></param>
		/// <param name="t"></param>
		public NodeNestedExecutable(
			CommandMetadata metadata,
			NodeContainer parent,
			IServiceProvider builder,
			Type t)
			: base(metadata, parent, builder, t)
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
		public override Node FindCommand(IArgumentPack pack)
		{
			var arg = pack.Peek().Unwrap()
				.ToLowerInvariant();

            if(Metadata.Identifiers == null || !Metadata.Identifiers.Any())
            {
                return null;
            }

            if(Metadata.Identifiers.All(x => x != arg))
            {
                return null;
            }

            pack.Take();
            var cmd = base.FindCommand(pack);
            if(cmd != null)
            {
                return cmd;
            }
            return this;
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
