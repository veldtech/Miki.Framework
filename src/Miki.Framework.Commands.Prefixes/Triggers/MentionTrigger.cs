namespace Miki.Framework.Commands.Prefixes.Triggers
{
    using System.Threading.Tasks;
    using Miki.Discord.Common;

    public class MentionTrigger : ITrigger
	{
		public async Task<string> CheckTriggerAsync(IContext context)
        {
            var packet = context.GetQuery();
            var index = packet.IndexOf('>');
            if(index == -1)
            {
                return null;
            }

            var prefix = packet;
            if(packet.Length > index + 1)
            {
                prefix = packet.Substring(0, index + 1);
            }

            if(!Mention.TryParse(prefix, out var mention))
			{
				return null;
			}
            
            var client = context.GetService<IDiscordClient>();
            var self = await client.GetSelfAsync();
			if(self.Id != mention.Id)
			{
				return null;
			}
            return prefix;
		}
	}
}
