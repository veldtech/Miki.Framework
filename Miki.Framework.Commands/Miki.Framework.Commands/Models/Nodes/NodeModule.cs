using System.Threading.Tasks;

namespace Miki.Framework.Commands.Nodes
{
    public class NodeModule : NodeContainer
    {
        public NodeModule(string name)
            : base(new CommandMetadata { Name = name })
        { }
        public NodeModule(string name, NodeContainer parent)
            : base(new CommandMetadata { Name = name }, parent)
        { }

        public override async Task RunAsync(IContext e)
        {
            if(!e.GetArgumentPack().CanTake)
            {
                return;
            }

            var arg = e.GetArgumentPack().Pack
                .Peek().ToLowerInvariant();
            foreach(var c in Children)
            {
                if(c.Metadata.Name.ToLowerInvariant() == arg)
                {
                    e.GetArgumentPack().Pack.Take();
                    await c.RunAsync(e);
                    break;
                }
            }
        }
    }
}
