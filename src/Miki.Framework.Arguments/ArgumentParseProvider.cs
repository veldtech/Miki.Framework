namespace Miki.Framework.Arguments
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ArgumentParseProvider
    {
		private static readonly List<IArgumentParser> parsers = new List<IArgumentParser>();

		public ArgumentParseProvider()
		{
			SeedAssembly(GetType().Assembly);
		}

		public object Peek(IArgumentPack p, Type type)
		{
			int cursor = p.Cursor;
			object output = parsers.Where(x => type.GetTypeInfo().IsAssignableFrom(x.OutputType))
				.Where(x => x.CanParse(p, type))
				.OrderByDescending(x => x.Priority)
				.FirstOrDefault()?
				.Parse(p, type);
			p.SetCursor(cursor);
			return output;
		}

		public T Peek<T>(IArgumentPack p)
			=> (T)Peek(p, typeof(T));

		public object Take(IArgumentPack p, Type type)
			=> parsers.Where(x => type.GetTypeInfo().IsAssignableFrom(x.OutputType))
				.Where(x => x.CanParse(p, type))
				.OrderByDescending(x => x.Priority)
				.FirstOrDefault()?
				.Parse(p, type);

		public T Take<T>(IArgumentPack p)
			=> (T)Take(p, typeof(T));

		public void SeedAssembly(Assembly a)
		{
            if (parsers.Count != 0)
            {
                return;
            }

            Type[] types = a.GetTypes()
                .Where(x => x.GetTypeInfo().IsClass && typeof(IArgumentParser).GetTypeInfo().IsAssignableFrom(x))
                .ToArray();

            foreach (var t in types)
            {
                IArgumentParser p = (IArgumentParser) Activator.CreateInstance(t);
                parsers.Add(p);
            }
        }
	}

}
