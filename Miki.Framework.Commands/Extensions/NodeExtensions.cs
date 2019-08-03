using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miki.Framework.Commands.Nodes;

namespace Miki.Framework.Commands
{
    public static class NodeExtensions
    {
        /// <summary>
        /// Validate if the <see cref="context"/> has all the requirements to execute the <see cref="executable"/>.
        /// </summary>
        /// <param name="executable">The executable.</param>
        /// <param name="context">The context.</param>
        /// <returns>True if the <see cref="context"/> has the requirements.</returns>
        public static Task<bool> ValidateRequirementsAsync(this IExecutable executable, IContext context)
        {
            if (executable is Node node)
            {
                return ValidateRequirementsAsync(node, context);
            }
            return Task.FromResult(true);
        }

        /// <summary>
        /// Validate if the <see cref="context"/> has all the requirements to the <see cref="node"/>.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="context">The context.</param>
        /// <returns>True if the <see cref="context"/> has the requirements.</returns>
        public static async Task<bool> ValidateRequirementsAsync(this Node node, IContext context)
        {
            foreach (var requirement in node.Attributes
                .OfType<ICommandRequirement>())
            {
                if (!(await requirement.CheckAsync(context)))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get all the nodes that extend <see cref="IExecutable"/> where the <see cref="context"/> has access to.
        /// </summary>
        /// <param name="nodeContainer">The node container.</param>
        /// <param name="context">The context.</param>
        /// <returns>All the identifiers within the node container.</returns>
        public static async Task<IEnumerable<Node>> GetAllExecutableAsync(this NodeContainer nodeContainer, IContext context)
        {
            #if NETSTANDARD2_1 || NETCOREAPP3_0
            #error This method should be rewritten using IAsyncEnumerable (https://github.com/dotnet/csharplang/blob/master/proposals/csharp-8.0/async-streams.md)
            #endif

            var executables = new List<Node>();

            foreach (var node in nodeContainer.Children)
            {
                if (!(await ValidateRequirementsAsync(node, context)))
                {
                    continue;
                }

                switch (node)
                {
                    case IExecutable executable:
                    {
                        executables.Add(node);
                        break;
                    }
                    case NodeContainer container:
                    {
                        executables.AddRange(await GetAllExecutableAsync(container, context));
                        break;
                    }
                }
            }
            return executables;
        }
    }
}
