using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Models
{
    public interface IChannel
    {
        Task<IMessage> CreateMessageAsync(string content);
    }
}
