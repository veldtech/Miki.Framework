namespace Miki.Framework
{
    public static class UlongExtensions
    {
        public static long ToDbLong(this ulong l)
        {
            unchecked
            {
                return (long)l;
            }
        }
    }
}