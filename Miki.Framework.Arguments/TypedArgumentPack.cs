using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Arguments
{
    public class TypedArgumentPack : ITypedArgumentPack
    {
        private readonly IArgumentPack _args;
        private readonly ArgumentParseProvider _parseProvider;

        public TypedArgumentPack(IArgumentPack pack, ArgumentParseProvider parseProvider)
        {
            _args = pack;
            _parseProvider = parseProvider;
        }

        public bool CanTake => _args.CanTake;

        public T Peek<T>()
            => _parseProvider.Peek<T>(_args);

        public T Take<T>()
            => _parseProvider.Take<T>(_args);
    }
}
