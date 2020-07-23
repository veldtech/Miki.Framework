using System.Threading.Tasks;

namespace Miki.Framework.Models
{
    public interface IChannel
    {
        Task<IMessage> CreateMessageAsync(string content);
    }
}
