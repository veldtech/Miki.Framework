namespace Miki.Framework.Commands.Prefixes
{
    using System;
    using System.Collections.Generic;
    using Miki.Framework.Commands.Prefixes.Triggers;

    public class PrefixCollection : List<ITrigger>
    {
        public ITrigger DefaultTrigger { get; private set; }

        public void AddDefault(ITrigger value)
        {
            if(DefaultTrigger != null)
            {
                throw new InvalidOperationException("Default trigger already set.");
            }
            DefaultTrigger = value;
            Add(value);
        }
    }
}