using System;

namespace Miki.Framework
{
	/// <summary>
	/// Extension methods for the ulong type.
	/// </summary>
    public static class UlongExtensions
	{
		/// <summary>
		/// Casts an ulong to a long.
		/// </summary>
		[Obsolete("Consider just casting it to a long.")]
		public static long ToDbLong(this ulong l)
		{
			unchecked
			{
				return (long)l;
			}
		}
	}
}