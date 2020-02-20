namespace Miki.Framework.Commands.Prefixes.Triggers
{
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Miki.Discord.Common;

    public class MentionTrigger : ITrigger
	{
		public async Task<string> CheckTriggerAsync(IContext context)
        {
            var packet = context.GetMessage();
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
            return result.Groups[1].Value;
		}
	}
}
