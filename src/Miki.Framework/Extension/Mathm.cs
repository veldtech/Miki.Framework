using System;

namespace Miki.Framework
{
    public class Mathm
	{
		public static float Clamp(float value, float min, float max)
		{
			return Math.Min(Math.Max(value, min), max);
		}

		public static int Clamp(int value, int min, int max)
		{
			return Math.Min(Math.Max(value, min), max);
		}

		public static bool IsEven(int value)
		{
			return (value & 1) == 0;
		}
	}
}