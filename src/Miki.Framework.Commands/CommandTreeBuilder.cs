using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Miki.Framework.Commands
{
    public class CommandTreeBuilder
	{
		private readonly List<Type> types = new List<Type>();

		public CommandTreeBuilder(IServiceCollection services)
		{
			Services = services;
        }
		
		public IServiceCollection Services { get; }
		
		public CommandTreeBuilder AddType(Type type)
		{
			types.Add(type);
			return this;
		}
		
        public CommandTreeBuilder AddAssembly(Assembly assembly)
        {
	        types.AddRange(assembly.GetTypes().Where(x => x.GetCustomAttribute<ModuleAttribute>() != null));
	        return this;
        }
        
        public CommandTree Build(IServiceProvider provider)
        {
	        var root = new CommandTree();
	        var compiler = ActivatorUtilities.CreateInstance<CommandTreeCompiler>(provider);

	        foreach (var type in types)
	        {
		        var module = compiler.LoadModule(type, root.Root);
		        
		        if (module != null)
		        {
			        root.Root.Children.Add(module);
		        }
	        }
			
	        return root;
        }

	}
}
