using System;
using System.Collections.Generic;

namespace Miki.Framework.Commands.Attributes
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
	public class CommandAttribute : Attribute
	{
        public string Name { get; }

        public IEnumerable<string> Aliases { get; }

        public CommandAttribute() { }
		public CommandAttribute(string name, params string[] aliases)
		{
            Name = name;
            Aliases = aliases;
		}

        public CommandMetadata AsMetadata()
        {
            return new CommandMetadata
            {
                Name = Name,
                Aliases = Aliases
            };
        }
	}
}