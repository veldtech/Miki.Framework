using System;
using System.Collections.Generic;
using System.Linq;

namespace Miki.Framework.Commands
{
	internal static class CommandTreeHelpers
	{
		internal static object CreateInstance(Type type, IServiceProvider app)
		{
			var defaultConstructor = type
				.GetConstructors()
				.FirstOrDefault();

			if(defaultConstructor != null
				&& defaultConstructor.GetParameters().Length != 0)
			{
				List<object> paramCollection = new List<object>();
				foreach(var p in defaultConstructor.GetParameters())
				{
					paramCollection.Add(app.GetService(p.ParameterType));
				}
				return Activator.CreateInstance(type, paramCollection.ToArray());
			}
			return Activator.CreateInstance(type);
		}
	}
}
