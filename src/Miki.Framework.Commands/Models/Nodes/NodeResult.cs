namespace Miki.Framework.Commands
{
    public readonly struct NodeResult
    {
        public NodeResult(Node node, int cursorPosition)
        {
            Node = node;
            CursorPosition = cursorPosition;
        }

        public Node Node { get; }
	    
        public int CursorPosition { get; }
    }
}