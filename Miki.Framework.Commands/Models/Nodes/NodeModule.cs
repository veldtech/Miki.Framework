using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Nodes
{
    public class NodeModule : NodeContainer
    {
        public NodeModule(string id)
            : this(id, null)
        { }
        public NodeModule(string id, NodeContainer parent)
            : base(new CommandMetadata { Identifiers = new[] { id } }, parent)
        { }
    }
}
