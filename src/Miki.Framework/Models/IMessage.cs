namespace Miki.Framework.Models
{
    public interface IMessage
	{
		object InnerMessage { get; }
		
		string Content { get; }
	}
}
