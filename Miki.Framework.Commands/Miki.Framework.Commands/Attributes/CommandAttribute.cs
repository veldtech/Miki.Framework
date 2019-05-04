using System;
using System.Collections.Generic;
using System.Linq;

namespace Miki.Framework.Commands.Attributes
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
	public class CommandAttribute : Attribute
	{
        public IEnumerable<string> Aliases { get; }

        public CommandAttribute() { }
		public CommandAttribute(params string[] aliases)
		{
            if(aliases.Count(x => string.IsNullOrWhiteSpace(x)) != 0)
            {
                throw new ArgumentNullException("Aliases",
                    "Alias cannot be empty or null.");
            }
            Aliases = aliases;
		}

        public CommandMetadata AsMetadata()
        {
            return new CommandMetadata
            {
                Identifiers = Aliases
            };
        }
	}
}