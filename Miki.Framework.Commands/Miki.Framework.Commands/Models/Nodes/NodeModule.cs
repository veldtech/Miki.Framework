using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Nodes
{
    public class NodeModule : NodeContainer
    {
        public NodeModule()
            : base(new CommandMetadata())
        { }
        public NodeModule(NodeContainer parent)
            : base(new CommandMetadata(), parent)
        { }
    }
}
