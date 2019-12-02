using System.Collections.Generic;

namespace Miki.Framework.Commands
{
	public struct CommandMetadata
	{
		/// <summary>
		/// Aliases for this command.
		/// </summary>
		public IReadOnlyCollection<string> Identifiers;
	}
}
