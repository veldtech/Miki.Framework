namespace Miki.Framework.Arguments.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SuffixedIntArgumentParser : IArgumentParser
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

		public bool CanParse(IArgumentPack pack, Type targetType)
		{
            if(targetType != typeof(int))
            {
				return false;
            }

			var value = pack.Peek();
            if(!value.HasValue)
            {
				return false;
            }

            var argument = value.Unwrap();
			return suffixes.ContainsKey(char.ToLowerInvariant(argument.Last()))
				&& int.TryParse(argument[..^1], out _);
		}

		public object Parse(IArgumentPack pack, Type targetType)
		{
			var value = pack.Take();
			return (int)(int.Parse(value[0..^1])
				* suffixes[char.ToLowerInvariant(value[^1])]);
		}
	}
}
