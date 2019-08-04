using System;
using System.Collections.Generic;
using System.Linq;

namespace Miki.Framework.Commands.Attributes
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
	public class CommandAttribute : Attribute
	{
		public IReadOnlyCollection<string> Aliases { get; }

		public CommandAttribute() { }
		public CommandAttribute(params string[] aliases)
		{
			if(aliases.Any(string.IsNullOrWhiteSpace))
			{
				throw new ArgumentNullException(nameof(aliases), "Alias cannot be empty or null.");
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