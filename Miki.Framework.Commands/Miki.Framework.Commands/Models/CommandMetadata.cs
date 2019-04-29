using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Commands
{
    public struct CommandMetadata
    {
        /// <summary>
        /// Current node ID.
        /// </summary>
        public string Name;

        /// <summary>
        /// Aliases for this command.
        /// </summary>
        public IEnumerable<string> Aliases;
    }
}
