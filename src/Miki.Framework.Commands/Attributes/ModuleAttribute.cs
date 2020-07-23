using System;

namespace Miki.Framework.Commands
{
	/// <summary>
	/// The intial command container object. Will initiate <see cref="CommandTree"/> fetching for this
	/// type.
	/// </summary>
    public class ModuleAttribute : Attribute
	{
		/// <summary>
		/// Unique ID of the module.
		/// </summary>
		public string Name { get; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ModuleAttribute.ModuleAttribute(string)'
		public ModuleAttribute(string name)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ModuleAttribute.ModuleAttribute(string)'
		{
			Name = name;
		}
	}
}