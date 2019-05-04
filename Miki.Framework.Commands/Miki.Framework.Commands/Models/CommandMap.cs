using Miki.Framework.Arguments;
using Miki.Framework.Commands.Attributes;
using Miki.Framework.Commands.Nodes;
using Miki.Framework.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
    public class CommandMap
    {
        public NodeContainer Root { get; }

        public CommandMap()
        {
            Root = new NodeRoot();
        }

        public Node GetCommand(IArgumentPack pack)
        {
            return Root.FindCommand(pack);
        }

        public static CommandMap FromAssembly(Assembly assembly)
        {
            var allTypes = assembly.GetTypes()
                .Where(x => x.GetCustomAttribute<ModuleAttribute>() != null);
            var root = new CommandMap();
            foreach (var t in allTypes)
            {
                root.Root.Children.Add(LoadModule(t, root.Root));
            }
            return root;
        }
       
        private static NodeContainer LoadModule(Type t, NodeContainer parent)
        {
            var moduleAttrib = t.GetCustomAttribute<ModuleAttribute>();
            if(moduleAttrib == null)
            {
                throw new InvalidOperationException("Modules must have a valid ModuleAttribute.");
            }

            NodeContainer module = new NodeModule(parent);
            module.Instance = CreateInstance(t);

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
        private static Node LoadCommand(Type t, NodeContainer parent)
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

            var multiCommand = new NodeNestedExecutable(commandAttrib.AsMetadata(), parent, null);
            AddRequirements(t, multiCommand);
            multiCommand.Instance = CreateInstance(t);

            var allCommands = t.GetNestedTypes()
                .Where(x => x.GetCustomAttribute<CommandAttribute>() != null);
            foreach (var c in allCommands)
            {
                multiCommand.Children.Add(LoadCommand(c, multiCommand));
            }

            var allSingleCommands = t.GetMethods()
                .Where(x => x.GetCustomAttribute<CommandAttribute>() != null);
            foreach (var c in allSingleCommands)
            {
                var attrib = c.GetCustomAttribute<CommandAttribute>();
                if (attrib.Aliases == null
                    || attrib.Aliases.Count() == 0)
                {
                    var node = LoadCommand(c, multiCommand);
                    if (node is IExecutable execNode)
                    {
                        multiCommand.SetDefaultExecution(async (e) 
                            => await execNode.RunAsync(e));
                    }
                }
                else
                {
                    multiCommand.Children.Add(LoadCommand(c, multiCommand));
                }
            }
            return multiCommand;
        }
        private static Node LoadCommand(MethodInfo m, NodeContainer parent)
        {
            var commandAttrib = m.GetCustomAttribute<CommandAttribute>();
            var command = new NodeExecutable(commandAttrib.AsMetadata(), parent);
            AddRequirements(m, command);

            if (m.ReturnType == typeof(Task)
                || m.ReturnType == typeof(Task<>)
                || m.ReturnType == typeof(ValueTask<>))
            {
                command.runAsync = async (e) =>
                {
                    await (m.Invoke(command.Parent.Instance, new[] { e }) as Task);
                };
            }
            else
            {
                command.runAsync = (e) =>
                {
                    m.Invoke(parent.Instance, new[] { e });
                    return Task.CompletedTask;
                };
            }
            return command;
        }

        private static void AddRequirements(ICustomAttributeProvider t, Node e)
        {
            if(e.Requirements == null)
            {
                return;
            }

            e.Requirements.AddRange(
                t.GetCustomAttributes(typeof(CommandRequirementAttribute), true)
                    .Select(x => x as ICommandRequirement));
        }

        private static object CreateInstance(Type type)
        {
            var defaultConstructor = type.GetConstructors()
                .FirstOrDefault(x => x.GetParameters().Count() == 0);
            if (defaultConstructor != null)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Module of type '{type.ToString()}' is missing a default constructor");
            }
        }
    }
}
