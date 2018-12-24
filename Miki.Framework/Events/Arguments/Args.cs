using Miki.Discord.Common;
using Miki.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
	public class Args
	{
		internal List<string> args;

		public int Count => args.Count;

		public Args(string a)
		{
			args = new List<string>();
			args.AddRange(a.Split(' '));
			args.RemoveAll(x => string.IsNullOrEmpty(x));
		}

		public bool Contains(string arg)
		{
			return args.Contains(arg);
		}

		public ArgObject First()
			=> Get(0);

		public ArgObject FirstOrDefault()
			=> (Count > 0) ? First() : null;

		public ArgObject Last()
			=> Get(Count - 1);

		public ArgObject LastOrDefault()
			=> (Count > 0) ? Last() : null;

		public ArgObject Get(int index)
		{
			if (index >= args.Count || index < 0)
				throw new ArgObjectNullException();

			return new ArgObject(args[index], index, this);
		}

		public ArgObject GetOrDefault(int index)
		{
			if (index >= args.Count || index < 0)
				return null;

			return Get(index);
		}

		public ArgObject Join()
			=> new ArgObject(string.Join(" ", args), 0, this);

		public void Remove(string value)
		{
			args.Remove(value);
		}

		public IEnumerable<ArgObject> Where(Func<string, bool> predicate)
		{
			List<ArgObject> allObjects = new List<ArgObject>();
			for (int i = 0; i < Count; i++)
			{
				if (predicate(Get(i).Argument))
				{
					allObjects.Add(Get(i));
				}
			}
			return allObjects;
		}

		public override string ToString()
		{
			return Join().Argument;
		}
	}

	public class ArgObject
	{
		public string Argument { get; private set; }

		private readonly Args args;
		private readonly int index;

		public bool IsLast
			=> (args.Count - 1 == index);

		public bool IsMention
			=> Regex.IsMatch(Argument, "<@(!?)\\d+>");

		public ArgObject(string argument, int index, Args a)
		{
			Argument = argument;
			this.index = index;
			args = a;
		}

		public int? TakeInt()
		{
			if (int.TryParse(Argument, out int s))
			{
				return s;
			}
			return null;
		}

		public bool? TakeBoolean()
		{
			if (bool.TryParse(Argument, out bool s))
			{
				return s;
			}
			return null;
		}

		public ArgObject TakeString()
		{
			if (Argument.StartsWith('"'))
			{
				List<string> toTake = new List<string>();
				ArgObject arg = this;

				while (true)
				{
					toTake.Add(arg.Argument);

					if(arg.Argument.EndsWith('"'))
					{
						break;
					}

					arg = arg.Next();

					if (arg == null)
					{
						throw new UnclosedDelimiterException("\"", Argument);
					}
				}
				return new ArgObject(string.Join(" ", toTake).TrimStart('"').TrimEnd('"'), arg.index, args);
			}
			else
			{
				return this;
			}
		}

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(Argument);
		}

		public IEnumerable<ArgObject> TakeWhile(Func<ArgObject, bool> func)
		{
			List<ArgObject> o = new List<ArgObject>();

			for (int i = index; i < args.args.Count; i++)
			{
				var current = args.Get(i);
				if (func(current))
				{
					o.Add(current);
				}
				else
				{
					break;
				}
			}

			return o;
		}

		public ArgObject TakeUntilEnd(int offset = 0)
		{
			ArgObject o = this;
			for (int i = index + 1; i < args.Count - offset; i++)
			{
				o.Argument += " " + args.Get(i).Argument;
			}
			return o;
		}

		public bool TryParseInt(out int i)
		{
			if (int.TryParse(Argument, out int x))
			{
				i = x;
				return true;
			}
			i = 0;
			return false;
		}

		public async Task<IDiscordGuildUser> GetUserAsync(IDiscordGuild guild)
		{
			if (string.IsNullOrWhiteSpace(Argument))
			{
				return null;
			}

			IDiscordGuildUser guildUser = null;
			if (IsMention)
			{
				guildUser = await guild.GetMemberAsync(ulong.Parse(Argument
					.TrimStart('<')
					.TrimStart('@')
					.TrimStart('!')
					.TrimEnd('>')));
			}
			else if (ulong.TryParse(Argument, out ulong id))
			{
				guildUser = await guild.GetMemberAsync(id);
			}
			else
			{
                var allUsers = await guild.GetMembersAsync();

                guildUser = allUsers.Where(x => x != null)
					.Where(x =>
					{
						if (x.Nickname != null)
						{
							if (x.Nickname.ToLowerInvariant() == Argument.ToLowerInvariant())
							{
								return true;
							}
						}
						else if (x.Username != null)
						{
							if (x.Username.ToLowerInvariant() == Argument.ToLowerInvariant())
							{
								return true;
							}
							else if (Argument == (x.Username + "#" + x.Discriminator).ToLowerInvariant())
							{
								return true;
							}
						}
						return false;
					})
					.FirstOrDefault();
			}

			if (guildUser == null)
			{
				throw new ArgObjectNullException();
			}

			if (guildUser.Id == 0)
			{
				throw new ArgObjectNullException();
			}

			return guildUser;
		}

		public ArgObject Next()
		{
			return args.GetOrDefault(index + 1);
		}
	}
}