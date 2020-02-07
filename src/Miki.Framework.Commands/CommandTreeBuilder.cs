using Miki.Framework.Commands.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Miki.Framework.Commands
{
	public class CommandTreeBuilder
	{
        [Obsolete("use 'CommandTreeBuilder::AddCommandBuildStep()' intead")]
		public event Action<NodeContainer, IServiceProvider> OnContainerLoaded;

		private readonly IServiceProvider services;

        private List<ICommandBuildStep> buildSteps;

		public CommandTreeBuilder(IServiceProvider services)
		{
			this.services = services;
        }

        public CommandTreeBuilder AddCommandBuildStep(ICommandBuildStep buildStep)
        {
            if(buildSteps == null)
            {
                buildSteps = new List<ICommandBuildStep>();
            }
            buildSteps.Add(buildStep);
            return this;
        }

		public CommandTree Create(Assembly assembly)
		{
			var allTypes = assembly.GetTypes()
				.Where(x => x.GetCustomAttribute<ModuleAttribute>() != null);
			var root = new CommandTree();
			foreach(var t in allTypes)
			{
				var module = LoadModule(t, root.Root);
				if(module != null)
				{
					root.Root.Children.Add(module);
				}
			}
			return root;
		}

		private NodeContainer LoadModule(Type t, NodeContainer parent)
		{
			var moduleAttrib = t.GetCustomAttribute<ModuleAttribute>();
			if(moduleAttrib == null)
			{
				throw new InvalidOperationException("Modules must have a valid ModuleAttribute.");
			}

			NodeContainer module = new NodeModule(moduleAttrib.Name, parent, services, t);
			OnContainerLoaded?.Invoke(module, services);

			var allCommands = t.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public)
				.Where(x => x.GetCustomAttribute<CommandAttribute>() != null);
			foreach(var c in allCommands)
			{
				module.Children.Add(LoadCommand(c, module));
			}

			var allSingleCommands = t.GetMethods()
				.Where(x => x.GetCustomAttribute<CommandAttribute>() != null);
			foreach(var c in allSingleCommands)
			{
				module.Children.Add(LoadCommand(c, module));
			}

			return module;
		}
		private Node LoadCommand(Type t, NodeContainer parent)
		{
			var commandAttrib = t.GetCustomAttribute<CommandAttribute>();
			if(commandAttrib == null)
			{
				throw new InvalidOperationException(
					$"Multi command of type '{t.ToString()}' must have a valid CommandAttribute.");
			}

			if(commandAttrib.Aliases?.Count() == 0)
			{
				throw new InvalidOperationException(
					$"Multi commands cannot have an invalid name.");
			}

			var multiCommand = new NodeNestedExecutable(commandAttrib.AsMetadata(), parent, services, t);
			OnContainerLoaded?.Invoke(multiCommand, services);

			var allCommands = t.GetNestedTypes()
				.Where(x => x.GetCustomAttribute<CommandAttribute>() != null);
			foreach(var c in allCommands)
			{
				multiCommand.Children.Add(LoadCommand(c, multiCommand));
			}

			var allSingleCommands = t.GetMethods()
				.Where(x => x.GetCustomAttribute<CommandAttribute>() != null);
			foreach(var c in allSingleCommands)
			{
				var attrib = c.GetCustomAttribute<CommandAttribute>();
				if(attrib.Aliases == null
					|| attrib.Aliases.Count() == 0)
				{
					var node = LoadCommand(c, multiCommand);
					if(node is IExecutable execNode)
					{
						multiCommand.SetDefaultExecution(async (e)
							=> await execNode.ExecuteAsync(e));
					}
				}
				else
				{
					multiCommand.Children.Add(LoadCommand(c, multiCommand));
				}
			}
			return multiCommand;
		}
		private Node LoadCommand(MethodInfo m, NodeContainer parent)
		{
			var commandAttrib = m.GetCustomAttribute<CommandAttribute>();
			var command = new NodeExecutable(commandAttrib.AsMetadata(), parent, m);

			if(m.ReturnType != typeof(Task))
			{
				throw new Exception("Methods with attribute 'Command' require to be Tasks.");
			}
			return command;
		}
	}
}
