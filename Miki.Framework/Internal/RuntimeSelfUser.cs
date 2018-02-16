using Discord;
using Miki.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Internal
{
	class RuntimeSelfUser : RuntimeUser, IDiscordSelfUser
	{
		public RuntimeSelfUser(ISelfUser user) : base(user)
		{ }

		public async Task ModifyAsync(Action<SelfUserData> modifiedData)
		{
			SelfUserData data = new SelfUserData();
			modifiedData(data);

			await (user as ISelfUser).ModifyAsync((x) =>
			{
				if(x.Username.IsSpecified)
					x.Username = data.Username;

				if (data.Image.HasValue)
					x.Avatar = new Image(data.Image.Value.Stream);
			});
		}
	}
}
