namespace Miki.Framework.Arguments.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class SuffixedIntArgumentParser : IArgumentParser
	{
		private readonly Dictionary<char, long> suffixes = new Dictionary<char, long>
		{
			{ 'k', 1000 },
			{ 'm', 1000000 },
			{ 'b', 1000000000 }
		};

		public Type OutputType
			=> typeof(int);

		public int Priority => 1;

		public bool CanParse(IArgumentPack pack)
		{
			var value = pack.Peek();
			return suffixes.ContainsKey(char.ToLowerInvariant(value.Last()))
				&& int.TryParse(value.Substring(0, value.Length - 1), out _);
		}

		public object Parse(IArgumentPack pack)
		{
			var value = pack.Take();
			return (int)(int.Parse(value.Substring(0, value.Length - 1))
				* suffixes[char.ToLowerInvariant(value[^1])]);
		}
	}
}
