using Discord;
using Miki.Common.Interfaces;
using Miki.Framework.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Common
{
    public class RuntimeGuild : IDiscordGuild, IProxy<IGuild>
    {
        public IGuild guild = null;

        public string AvatarUrl 
			=> guild.IconUrl;

		public ulong Id 
			=> guild.Id;

		public string Name 
			=> guild.Name;

		public ulong OwnerId 
			=> guild.OwnerId;

		public List<IDiscordRole> Roles
			=> guild.Roles.Select(x => new RuntimeRole(x)).Cast<IDiscordRole>().ToList();

		public RuntimeGuild(IGuild g)
        {
            guild = g;
        }

		public async Task<int> GetChannelCountAsync()
			=> (await guild.GetChannelsAsync()).Count;

		public async Task<List<IDiscordMessageChannel>> GetChannelsAsync()
		=> (await guild.GetChannelsAsync())
			.Select(x => new RuntimeMessageChannel(x))
			.Cast<IDiscordMessageChannel>()
			.ToList();

		public async Task<IDiscordUser> GetCurrentUserAsync()
			=> new RuntimeUser(await guild.GetCurrentUserAsync());

		public async Task<IDiscordMessageChannel> GetDefaultChannelAsync()
			=> new RuntimeMessageChannel(await guild.GetDefaultChannelAsync());

		public async Task<IDiscordUser> GetOwnerAsync()
			=> new RuntimeUser(await guild.GetOwnerAsync());

		public async Task<int> GetUserCountAsync()
			=> (await guild.GetUsersAsync()).Count;

		public async Task<int> GetVoiceChannelCountAsync()
			=> (await guild.GetVoiceChannelsAsync()).Count;

		public async Task<IDiscordUser> GetUserAsync(ulong userId)
		{
			var user = await guild.GetUserAsync(userId);

			if(user != null)
				return new RuntimeUser(user);
			return null;
		}
		public async Task<IDiscordUser> GetUserAsync(string username)
			=> (await GetUsersAsync())
				.FirstOrDefault(x => x.Nickname == username || x.Username == username || $"{x.Username}#{x.Discriminator}" == username);

		public async Task<List<IDiscordUser>> GetUsersAsync()
			=> (await guild.GetUsersAsync())
				.Select(x => new RuntimeUser(x))
				.Cast<IDiscordUser>()
				.ToList();

        public IDiscordRole GetRole(ulong roleId)
			=> new RuntimeRole(guild.GetRole(roleId));

		public IGuild ToNativeObject()
			=> guild;
	}
}