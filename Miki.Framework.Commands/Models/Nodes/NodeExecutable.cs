﻿namespace Miki.Framework.Commands.Nodes
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public delegate Task CommandDelegate(IContext c);

	public class NodeExecutable : Node, IExecutable
	{
		private readonly CommandDelegate runAsync;

		public NodeExecutable(CommandMetadata metadata, NodeContainer parent, MethodInfo method)
			: base(metadata, parent, method)
		{
			runAsync = (CommandDelegate)Delegate.CreateDelegate(
				typeof(CommandDelegate), parent.Instance, method, true);
		}

		public async ValueTask ExecuteAsync(IContext e)
		{
			if(runAsync == null)
			{
				throw new InvalidProgramException("Invalid method bindings for command " + ToString());
			}

			foreach(var req in Attributes
				.OfType<ICommandRequirement>())
			{
				if(!await req.CheckAsync(e))
				{
					await req.OnCheckFail(e);
					return;
				}
			}
			await runAsync(e);
		}
	}
}
