using System;
using System.Collections.Generic;
using System.Text;

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
