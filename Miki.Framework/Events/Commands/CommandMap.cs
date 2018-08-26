using Miki.Framework.Events.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
    public class CommandMap
    {
		public IReadOnlyList<CommandEvent> Commands
			=> commandCache.Values.ToList();

		public IReadOnlyList<Module> Modules 
			=> modulesLoaded;

		public event Action<Module> OnModuleLoaded;

		private List<Module> modulesLoaded = new List<Module>();
		private Dictionary<string, CommandEvent> commandCache = new Dictionary<string, CommandEvent>();

		public CommandMap()
		{
		}

		public void AddCommand(CommandEvent command)
		{
			foreach (string a in command.Aliases)
			{
				commandCache.Add(a.ToLower(), command);
			}
			commandCache.Add(command.Name.ToLower(), command);
		}

		public void AddModule(Module module)
			=> modulesLoaded.Add(module);

		public CommandEvent GetCommandEvent(string value)
		{
			if(commandCache.TryGetValue(value, out var cmd))
			{
				return cmd;
			}
			return null;
		}

		public void Install(EventSystem system)
		{
			foreach (var m in modulesLoaded)
			{
				m.Install(system);
			}
		}

		public void RegisterAttributeCommands(Assembly assembly = null)
		{
			Bot b = Bot.Instance;

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

				try
				{
					instance = Activator.CreateInstance(Type.GetType(m.AssemblyQualifiedName), newModule, b);
				}
				catch
				{
					try
					{
						instance = Activator.CreateInstance(Type.GetType(m.AssemblyQualifiedName), newModule);
					}
					catch
					{
						instance = Activator.CreateInstance(Type.GetType(m.AssemblyQualifiedName));
					}
				}

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
					CommandEvent newEvent = new CommandEvent();
					CommandAttribute commandAttribute = x.GetCustomAttribute<CommandAttribute>();

					newEvent = commandAttribute.command;
					newEvent.ProcessCommand = async (context) => await (Task)x.Invoke(instance, new object[] { context });
					newEvent.Module = newModule;

					CommandEvent foundCommand = newModule.Events.Find(c => c.Name == newEvent.Name);

					if (foundCommand != null)
					{
						foundCommand.Default(newEvent.ProcessCommand);
					}
					else
					{
						newModule.AddCommand(newEvent);
					}

					foreach(var a in newEvent.Aliases)
					{
						commandCache.Add(a.ToLower(), newEvent);
					}
					commandCache.Add(newEvent.Name.ToLower(), newEvent);
				}

				var services = m.GetProperties().Where(x => x.GetCustomAttributes<ServiceAttribute>().Count() > 0).ToArray();

				foreach(var s in services)
				{
					BaseService service = Activator.CreateInstance(s.PropertyType, true) as BaseService;
					var attrib = s.GetCustomAttribute<ServiceAttribute>();

					service.Name = attrib.Name;
					newModule.Services.Add(service);
				}

				OnModuleLoaded?.Invoke(newModule);

				modulesLoaded.Add(newModule);
			}
		}

		public void RemoveCommand(CommandEvent command)
		{
			commandCache.Remove(command.Name.ToLower());
			foreach(var c in command.Aliases)
			{
				commandCache.Remove(c.ToLower());
			}
		}

		public static CommandMap CreateFromAssembly(Assembly assembly = null)
		{
			CommandMap map = new CommandMap();
			map.RegisterAttributeCommands(assembly);
			return map;
		}
	}
}
