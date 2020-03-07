﻿namespace Miki.Framework.Commands.Nodes
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'CommandDelegate'
    public delegate Task CommandDelegate(IContext c);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'CommandDelegate'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'NodeExecutable'
	public class NodeExecutable : Node, IExecutable
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'NodeExecutable'
	{
		private readonly CommandDelegate runAsync;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'NodeExecutable.NodeExecutable(CommandMetadata, NodeContainer, MethodInfo)'
		public NodeExecutable(CommandMetadata metadata, NodeContainer parent, MethodInfo method)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'NodeExecutable.NodeExecutable(CommandMetadata, NodeContainer, MethodInfo)'
			: base(metadata, parent, method)
		{
			runAsync = (CommandDelegate)Delegate.CreateDelegate(
				typeof(CommandDelegate), parent.Instance, method, true);
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'NodeExecutable.ExecuteAsync(IContext)'
		public async ValueTask ExecuteAsync(IContext e)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'NodeExecutable.ExecuteAsync(IContext)'
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
