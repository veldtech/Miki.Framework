namespace Miki.Framework.Commands
{
    using System;
    using Miki.Framework.Commands.Nodes;

    public interface ICommandBuildStep
    {
        NodeModule BuildModule(NodeModule module, IServiceProvider provider);

        Node BuildNode(Node node, IServiceProvider provider);
    }
}
