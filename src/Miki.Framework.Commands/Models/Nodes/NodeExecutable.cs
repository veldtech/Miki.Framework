﻿using System.Linq.Expressions;
 using Miki.Framework.Models;

 namespace Miki.Framework.Commands.Nodes
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public delegate Task CommandDelegate(IContext c);

	public class NodeExecutable : Node, IExecutable
	{
		private readonly CommandDelegate runAsync;

		public NodeExecutable(CommandMetadata metadata, NodeContainer parent, CommandDelegate commandDelegate)
			: base(metadata, parent, parent.Type)
		{
			runAsync = commandDelegate;
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
