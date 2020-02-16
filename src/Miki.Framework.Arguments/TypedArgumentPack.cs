namespace Miki.Framework.Arguments
{
	public class TypedArgumentPack : ITypedArgumentPack
	{
		private readonly ArgumentParseProvider parseProvider;

		public TypedArgumentPack(IArgumentPack pack, ArgumentParseProvider parseProvider)
		{
			Pack = pack;
			this.parseProvider = parseProvider;
		}

		public bool CanTake => Pack.CanTake;

		public IArgumentPack Pack { get; }

		public void Skip()
		{
			if(Pack.CanTake)
			{
				Pack.SetCursor(Pack.Cursor + 1);
			}
		}

		public bool Peek<T>(out T value)
		{
			if(!CanTake)
			{
				value = default;
				return false;
			}

			var output = parseProvider.Peek(Pack, typeof(T));
			if(output == null)
			{
				value = default;
				return false;
			}
			value = (T)output;
			return true;
		}

		public bool Take<T>(out T value)
		{
			if(!CanTake)
			{
				value = default;
				return false;
			}

			var output = parseProvider.Take(Pack, typeof(T));
			if(output == null)
			{
				value = default;
				return false;
			}
			value = (T)output;
			return true;
		}

		public override string ToString()
		{
			return string.Join(" ", Pack);
		}
	}
}