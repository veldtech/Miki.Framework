namespace Miki.Framework.Arguments
{
	public static class StringUtils
	{
		public static bool StartsWithAny(this string str, params string[] checks)
		{
			foreach(var s in checks)
			{
				if(str.StartsWith(s))
				{
					return true;
				}
			}
			return false;
		}

		public static bool EndsWithAny(this string str, params string[] checks)
		{
			foreach(var s in checks)
			{
				if(str.EndsWith(s))
				{
					return true;
				}
			}
			return false;
		}
	}
}
