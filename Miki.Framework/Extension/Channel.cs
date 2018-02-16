using Miki.Framework;
using System;
using System.Threading.Tasks;

namespace Discord
{
    public static class ChannelExtension
    {
         private static async Task DeleteMessage(IUserMessage message, int seconds)
        {
            await Task.Delay(seconds * 1000);
            await message.DeleteAsync();
        }
    }
}