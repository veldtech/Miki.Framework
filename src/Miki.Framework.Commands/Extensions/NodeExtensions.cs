namespace Miki.Framework.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'NodeExtensions'
    public static class NodeExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'NodeExtensions'
	{
		
#pragma warning disable CS1574 // XML comment has cref attribute 'context' that could not be resolved
#pragma warning disable CS1574 // XML comment has cref attribute 'context' that could not be resolved
#pragma warning disable CS1574 // XML comment has cref attribute 'executable' that could not be resolved
/// <summary>
		/// Validate if the <see cref="context"/> has all the requirements to execute the <see cref="executable"/>.
		/// </summary>
		/// <param name="executable">The executable.</param>
		/// <param name="context">The context.</param>
		/// <returns>True if the <see cref="context"/> has the requirements.</returns>
		public static Task<bool> ValidateRequirementsAsync(this IExecutable executable, IContext context)
#pragma warning restore CS1574 // XML comment has cref attribute 'executable' that could not be resolved
#pragma warning restore CS1574 // XML comment has cref attribute 'context' that could not be resolved
#pragma warning restore CS1574 // XML comment has cref attribute 'context' that could not be resolved
		{
			if(executable is Node node)
			{
				return ValidateRequirementsAsync(node, context);
			}
			return Task.FromResult(true);
		}

		
#pragma warning disable CS1574 // XML comment has cref attribute 'context' that could not be resolved
#pragma warning disable CS1574 // XML comment has cref attribute 'context' that could not be resolved
#pragma warning disable CS1574 // XML comment has cref attribute 'node' that could not be resolved
/// <summary>
		/// Validate if the <see cref="context"/> has all the requirements to the <see cref="node"/>.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="context">The context.</param>
		/// <returns>True if the <see cref="context"/> has the requirements.</returns>
		public static async Task<bool> ValidateRequirementsAsync(this Node node, IContext context)
#pragma warning restore CS1574 // XML comment has cref attribute 'node' that could not be resolved
#pragma warning restore CS1574 // XML comment has cref attribute 'context' that could not be resolved
#pragma warning restore CS1574 // XML comment has cref attribute 'context' that could not be resolved
		{
			foreach(var requirement in node.Attributes
				.OfType<ICommandRequirement>())
			{
				if(!(await requirement.CheckAsync(context)))
				{
					return false;
				}
			}
			return true;
		}

		
#pragma warning disable CS1574 // XML comment has cref attribute 'context' that could not be resolved
/// <summary>
		/// Get all the nodes that extend <see cref="IExecutable"/> where the <see cref="context"/> has access to.
		/// </summary>
		/// <param name="nodeContainer">The node container.</param>
		/// <param name="context">The context.</param>
		/// <returns>All the identifiers within the node container.</returns>
		public static async IAsyncEnumerable<Node> GetAllExecutableAsync(
#pragma warning restore CS1574 // XML comment has cref attribute 'context' that could not be resolved
            this NodeContainer nodeContainer, 
            IContext context)
		{
			foreach(var node in nodeContainer.Children)
			{
				if(!await ValidateRequirementsAsync(node, context))
				{
					continue;
				}

				switch(node)
				{
					case IExecutable _:
					{
						yield return node;
						break;
					}
					case NodeContainer container:
					{
                        await foreach (var i in GetAllExecutableAsync(container, context))
                        {
                            yield return i;
                        }
						break;
					}
				}
			}
        }
	}
}
