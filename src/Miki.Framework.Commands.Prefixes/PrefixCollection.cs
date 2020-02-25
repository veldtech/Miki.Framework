namespace Miki.Framework.Commands.Prefixes
{
    using System;
    using System.Collections.Generic;
    using Miki.Framework.Commands.Prefixes.Triggers;

    public class PrefixCollectionBuilder
    {
        internal readonly List<ITrigger> triggers = new List<ITrigger>();
        internal ITrigger defaultTriggerRef;

        public PrefixCollectionBuilder Add(ITrigger trigger)
        {
            if(triggers.Contains(trigger))
            {
                throw new InvalidOperationException("You cannot add duplicate triggers.");
            }
            triggers.Add(trigger);
            return this;
        }

        public PrefixCollectionBuilder AddAsDefault(ITrigger trigger)
        {
            Add(trigger);
            defaultTriggerRef = trigger;
            return this;
        }

        public PrefixCollection Build()
        {
            if(defaultTriggerRef == null)
            {
                throw new InvalidOperationException(
                    "No default trigger set. This can cause unintential behaviour");
            }

            var collection = new PrefixCollection();
            collection.AddRange(triggers);
            collection.SetDefault(defaultTriggerRef);
            return collection;
        }
    }

    public class PrefixCollection : List<ITrigger>
    {
        public ITrigger DefaultTrigger { get; private set; }
        
        public void AddDefault(ITrigger value)
        {
            Add(value);
            SetDefault(value);
        }

        public void SetDefault(ITrigger value)
        {
            if(!Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            DefaultTrigger = value;
        }
    }
}