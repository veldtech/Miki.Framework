namespace Miki.Framework.Arguments.Parsers
{
    using System;
    using System.Collections.Generic;

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
			var arg = pack.Peek();
			if(arg.StartsWithAny("\"", "“", "”"))
			{
				List<string> allItems = new List<string>();
				do
				{
					allItems.Add(pack.Take());
				} while(!allItems[^1].EndsWithAny("\"", "“", "”"));
				return string.Join(" ", allItems).TrimStart('"', '“', '”').TrimEnd('"', '“', '”');
			}
			else
			{
				return pack.Take();
			}
		}
	}

}
