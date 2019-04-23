using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Commands.Models
{
    public struct CommandMetadata
    {
        /// <summary>
        /// Current Node's name.
        /// </summary>
        public string Name;

        /// <summary>
        /// Aliases for this command.
        /// </summary>
        public IEnumerable<string> Aliases;
    }
}
