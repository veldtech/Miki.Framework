namespace Miki.Framework.Models
{
    using System.Threading.Tasks;

    public interface IChannel
    {
        Task<IMessage> CreateMessageAsync(string content);
    }
}
