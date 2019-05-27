using Miki.Discord.Common;
using Miki.Framework.Commands.Pipelines;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Triggers
{
    public class MentionTrigger : ITrigger<IDiscordMessage>
    {
        public async Task<string> CheckTriggerAsync(IMutableContext context, IDiscordMessage packet)
        {
            var result = Regex.Match(packet.Content, "^<@!?(\\d+)> ");
            if(!result.Success)
            {
                return null;
            }

            var guild = context.GetGuild();
            if((await guild.GetSelfAsync()).Id.ToString() 
                != result.Groups[1].Value)
            {
                return null;
            }

            context.GetQuery().RemoveAt(0);
            return result.Groups[1].Value;
        }
    }
}
