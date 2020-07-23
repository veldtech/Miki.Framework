using System;
using Miki.Framework.Commands.Nodes;

namespace Miki.Framework.Commands
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ICommandBuildStep'
    public interface ICommandBuildStep
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ICommandBuildStep'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ICommandBuildStep.BuildModule(NodeModule, IServiceProvider)'
        NodeModule BuildModule(NodeModule module, IServiceProvider provider);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ICommandBuildStep.BuildModule(NodeModule, IServiceProvider)'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ICommandBuildStep.BuildNode(Node, IServiceProvider)'
        Node BuildNode(Node node, IServiceProvider provider);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ICommandBuildStep.BuildNode(Node, IServiceProvider)'
    }
}
