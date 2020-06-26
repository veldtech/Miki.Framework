using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Miki.Framework.Commands.Nodes;
using Miki.Framework.Hosting;
using Miki.Framework.Models;

namespace Miki.Framework.Commands
{
    internal class CommandTreeCompiler
    {
	    private static readonly PropertyInfo TaskCompleteProperty = typeof(Task).GetProperty(nameof(Task.CompletedTask));

	    private readonly IReadOnlyList<IParameterProvider> parameterProviders;

	    public CommandTreeCompiler(IEnumerable<IParameterProvider> parameterProviders)
	    {
		    this.parameterProviders = parameterProviders.ToArray();
	    }

	    public NodeContainer LoadModule(Type t, NodeContainer parent)
		{
			var moduleAttrib = t.GetCustomAttribute<ModuleAttribute>();
			if(moduleAttrib == null)
			{
				throw new InvalidOperationException("Modules must have a valid ModuleAttribute.");
			}

			NodeContainer module = new NodeModule(moduleAttrib.Name, parent, t);

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
					$"Multi command of type '{t}' must have a valid CommandAttribute.");
			}

			if(commandAttrib.Aliases?.Count == 0)
			{
				throw new InvalidOperationException(
					$"Multi commands cannot have an invalid name.");
			}

			var multiCommand = new NodeNestedExecutable(commandAttrib.AsMetadata(), parent, t);

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
			var command = new NodeExecutable(commandAttrib.AsMetadata(), parent, CreateDelegate(parent.Type, m));

			if(m.ReturnType != typeof(Task))
			{
				throw new Exception("Methods with attribute 'Command' require to be Tasks.");
			}
			return command;
		}
		
		private CommandDelegate CreateDelegate(Type type, MethodInfo methodInfo)
		{
			var context = Expression.Parameter(typeof(IContext));
			var builder = new ParameterBuilder(context);
			var constructors = type.GetConstructors();
			var module = constructors.Length switch
			{
				0 => Expression.New(type),
				1 => Expression.New(constructors[0], GetParameterValues(builder, constructors[0])),
				_ => throw new NotSupportedException($"The module {type} has multiple constructors")
			};
			var parameterValues = GetParameterValues(builder, methodInfo);
			var returnType = methodInfo.ReturnType;
			Expression result = Expression.Call(module, methodInfo, parameterValues);
			
			var returnTarget = Expression.Label(typeof(Task));
			var taskComplete = Expression.Property(null, TaskCompleteProperty);

			Expression[] callMethod;
			
			if (returnType == typeof(void))
			{
				callMethod = new[]
				{
					result,
					Expression.Return(returnTarget, taskComplete),
					Expression.Label(returnTarget, taskComplete)
				};
			}
			else if (returnType == typeof(Task))
			{
				callMethod = new Expression[]
				{
					Expression.Return(returnTarget, result),
					Expression.Label(returnTarget, taskComplete)
				};
			}
			else
			{
				throw new InvalidOperationException($"Method {type.Name}.{methodInfo.Name} should return Task or void.");
			}
			
			result = Expression.Block(builder.Variables.Values, builder.Initializers.Union(callMethod));

			var lambda = Expression.Lambda<CommandDelegate>(result, context);
			
			return lambda.Compile();
		}

		private Expression[] GetParameterValues(ParameterBuilder builder, MethodBase methodInfo)
		{
			var parameters = methodInfo.GetParameters();
			var parameterValues = new Expression[parameters.Length];

			for (var i = 0; i < parameters.Length; i++)
			{
				var paramType = parameters[i].ParameterType;
				var provider = parameterProviders.FirstOrDefault(p => paramType.IsAssignableFrom(p.ParameterType));
				Expression paramExpression;

				if (provider != null)
				{
					paramExpression = provider.Provide(builder);
				}
				else if (paramType == typeof(IContext))
				{
					paramExpression = builder.Context;
				}
				else if (paramType == typeof(IMessage))
				{
					paramExpression = Expression.Property(builder.Context, nameof(IContext.Message));
				}
				else
				{
					paramExpression = builder.GetService(paramType);
				}

				parameterValues[i] = paramExpression;
			}

			return parameterValues;
		}
    }
}