using Miki.Framework.Commands;
using Miki.Framework.Commands.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
    public class NodeMap
    {
		public event Action<Module> OnModuleLoaded;

		public INode RootNode;

		public NodeMap()
		{
			RootNode = new ModuleNode();
		}

		public void RegisterAttributeCommands(Assembly assembly = null)
		{
			if(assembly == null)
			{
				assembly = Assembly.GetEntryAssembly();
			}

			var modules = assembly.GetTypes()
				.Where(m => m.GetCustomAttributes<ModuleAttribute>().Count() > 0)
				.ToArray();

			foreach (var m in modules)
			{
				object instance = null;

				Module newModule = new Module(instance);

				instance = Activator.CreateInstance(Type.GetType(m.AssemblyQualifiedName), newModule);

				newModule.SetInstance(instance);

				ModuleAttribute mAttrib = m.GetCustomAttribute<ModuleAttribute>();
				newModule.Name = mAttrib.module.Name.ToLower();
				newModule.Nsfw = mAttrib.module.Nsfw;
				newModule.CanBeDisabled = mAttrib.module.CanBeDisabled;

				var methods = m.GetMethods()
					.Where(t => t.GetCustomAttributes<CommandAttribute>().Count() > 0)
					.ToArray();

				foreach (var x in methods)
				{

				}

				OnModuleLoaded?.Invoke(newModule);
			}
		}

		public static NodeMap CreateFromAssembly(Assembly assembly = null)
		{
			NodeMap map = new NodeMap();
			map.RegisterAttributeCommands(assembly);
			return map;
		}
	}
}
