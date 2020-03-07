namespace Miki.Framework.Arguments.Parsers
{
    using System;

    public class EnumArgumentParser : IArgumentParser
    {
        /// <inheritdoc />
        public Type OutputType => typeof(Enum);

        /// <inheritdoc />
        public int Priority => 1;

        /// <inheritdoc />
        public bool CanParse(IArgumentPack pack, Type targetType)
        {
            return Enum.TryParse(targetType, pack.Peek(), true, out _);
        }

        /// <inheritdoc />
        public object Parse(IArgumentPack pack, Type targetType)
        {
            return Enum.Parse(targetType, pack.Take(), true);
        }
    }
}
