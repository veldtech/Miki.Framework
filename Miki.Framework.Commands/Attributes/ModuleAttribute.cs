using System;

namespace Miki.Framework.Commands.Attributes
{
	public class ModuleAttribute : Attribute
	{
		public string Name { get; }

		public ModuleAttribute(string name)
		{
			Name = name;
		}
	}
}