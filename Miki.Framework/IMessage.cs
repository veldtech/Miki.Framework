using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework
{
    public interface IChannel
    {
        Task<IMessage> CreateMessageAsync(string content);
    }

    public interface IMessage
    {
        Task DeleteAsync();

        Task<IChannel> GetChannelAsync();

        Task<IMessage> ModifyAsync(string content);
    }
}
