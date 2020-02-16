namespace Miki.Framework.Arguments.Parsers
{
    using System;

    public class BooleanArgumentParser : IArgumentParser
	{
		public Type OutputType
			=> typeof(bool);

		public int Priority => 1;

        public bool CanParse(IArgumentPack pack, Type targetType)
        {
            if(targetType != typeof(bool))
            {
                return false;
            }
            return bool.TryParse(pack.Peek(), out _) 
                   || int.TryParse(pack.Peek(), out _);
        }

        public object Parse(IArgumentPack pack, Type targetType)
		{
			var item = pack.Take();
			if(bool.TryParse(item, out bool valueBool))
			{
				return valueBool;
			}

            if(int.TryParse(item, out int valueInt))
            {
                return valueInt > 0;
            }
            return null;
		}
	}
}
