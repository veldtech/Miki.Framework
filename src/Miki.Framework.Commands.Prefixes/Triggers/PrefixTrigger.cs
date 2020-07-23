using System.Linq;
using System;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Prefixes.Triggers
{
    public class PrefixTrigger : ITrigger
	{
		public string Value { get; internal set; }

		public Func<IContext, Task> OnTriggerReceived { get; set; }

		public PrefixTrigger(string value)
		{
			Value = value;
        }

		public async Task<string> CheckTriggerAsync(IContext e)
		{
			var query = e.GetQuery();
            if(!query.Any())
            {
				return null;
            }

			if(!query.StartsWith(Value))
			{
				return null;
			}

			if(OnTriggerReceived != null)
			{
				await OnTriggerReceived(e);
			}
			return Value;
		}
	}
}