using System;
using Miki.Framework.Commands.Nodes;

namespace Miki.Framework.Commands
{
    public interface ICommandBuildStep
    {
        NodeModule BuildModule(NodeModule module, IServiceProvider provider);

        Node BuildNode(Node node, IServiceProvider provider);
    }
}
