using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Miki.Framework.Arguments
{
	public class ArgumentParseProvider
	{
		private readonly List<IArgumentParser> _parsers = new List<IArgumentParser>();

		public ArgumentParseProvider()
		{
		}

        public object Peek(IArgumentPack p, Type type)
        {
            int cursor = p.Cursor;
            object output = _parsers.Where(x => type.GetTypeInfo().IsAssignableFrom(x.OutputType))
                .Where(x => x.CanParse(p))
                .OrderByDescending(x => x.Priority)
                .FirstOrDefault()
                .Parse(p);
            p.SetCursor(cursor);
            return output;
        }

        public T Peek<T>(IArgumentPack p)
            => (T)Peek(p, typeof(T));

        public object Take(IArgumentPack p, Type type)
            => _parsers.Where(x => type.GetTypeInfo().IsAssignableFrom(x.OutputType))
                .Where(x => x.CanParse(p))
                .OrderByDescending(x => x.Priority)
                .FirstOrDefault()
                .Parse(p);

        public T Take<T>(IArgumentPack p)
            => (T)Take(p, typeof(T));

		public void SeedAssembly(Assembly a)
		{
			Type[] allParsers = a.GetTypes()
				.Where(x => x.GetTypeInfo().IsClass && typeof(IArgumentParser).GetTypeInfo().IsAssignableFrom(x))
				.ToArray();

			foreach (var t in allParsers)
			{
				IArgumentParser p = (IArgumentParser)Activator.CreateInstance(t);
				_parsers.Add(p);
			}
		}
	}

}
