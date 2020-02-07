namespace Miki.Framework.Commands
{
    using System;

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

		public ModuleAttribute(string name)
		{
			Name = name;
		}
	}
}