using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miki.Framework.Arguments
{
    internal class SuffixedIntArgumentParser : IArgumentParser
    {
        private Dictionary<char, long> _suffixes = new Dictionary<char, long>
        {
            { 'k', 1000 },
            { 'm', 1000000 },
            { 'b', 1000000000 }
        };

        public Type OutputType
            => typeof(int);

        public int Priority => 1;

        public bool CanParse(IArgumentPack pack)
        {
            var value = pack.Peek();
            return _suffixes.ContainsKey(char.ToLowerInvariant(value.Last())) 
                && int.TryParse(value.Substring(0, value.Length - 1), out _);
        }

        public object Parse(IArgumentPack pack)
        {
            var value = pack.Take();
            return (int)(int.Parse(value.Substring(0, value.Length - 1)) 
                * _suffixes[char.ToLowerInvariant(value[value.Length - 1])]);
        }
    }
}
