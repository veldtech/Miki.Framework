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

        public IArgumentPack Pack => _args;

        public void Skip()
        {
            if (_args.CanTake)
            {
                _args.SetCursor(_args.Cursor + 1);
            }
        }

        public bool Peek<T>(out T value)
        {
            if(!CanTake)
            {
                value = default(T);
                return false;
            }

            var output = _parseProvider.Peek(_args, typeof(T));
            if (output == null)
            {
                value = default(T);
                return false;
            }
            value = (T)output;
            return true;
        }

            public bool Take<T>(out T value)
        {
            if (!CanTake)
            {
                value = default(T);
                return false;
            }

            var output = _parseProvider.Take(_args, typeof(T));
            if(output == null)
            {
                value = default(T);
                return false;
            }
            value = (T)output;
            return true;
        }

        public override string ToString()
        {
            return string.Join(" ", _args);
        }
    }
}
