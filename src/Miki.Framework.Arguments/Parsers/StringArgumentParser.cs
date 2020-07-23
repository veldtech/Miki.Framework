using System;
using System.Collections.Generic;

namespace Miki.Framework.Arguments.Parsers
{
    public class StringArgumentParser : IArgumentParser
	{
		public Type OutputType
			=> typeof(string);

		public int Priority
			=> 0;

		public bool CanParse(IArgumentPack pack, Type targetType)
			=> true;

        public object Parse(IArgumentPack pack, Type targetType)
        {
            var arg = pack.Peek().Unwrap();
            if(!arg.StartsWithAny("\"", "“", "”"))
            {
                return pack.Take();
            }

            var allItems = new List<string>();
            do
            {
                allItems.Add(pack.Take());
            } while(!allItems[^1].EndsWithAny("\"", "“", "”"));

            return string.Join(" ", allItems).TrimStart('"', '“', '”').TrimEnd('"', '“', '”');
        }
    }
}
