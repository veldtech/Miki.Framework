﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Miki.Framework.Utils
{
    internal class CopyOnWriteDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> sourceDictionary;
        private readonly IEqualityComparer<TKey> comparer;
        private IDictionary<TKey, TValue> innerDictionary;

        public CopyOnWriteDictionary(
            IDictionary<TKey, TValue> sourceDictionary,
            IEqualityComparer<TKey> comparer)
        {
            if (sourceDictionary == null)
            {
                throw new ArgumentNullException(nameof(sourceDictionary));
            }

            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            this.sourceDictionary = sourceDictionary;
            this.comparer = comparer;
        }

        private IDictionary<TKey, TValue> ReadDictionary
        {
            get
            {
                return innerDictionary ?? sourceDictionary;
            }
        }

        private IDictionary<TKey, TValue> WriteDictionary
        {
            get
            {
                if (innerDictionary == null)
                {
                    innerDictionary = new Dictionary<TKey, TValue>(sourceDictionary,
                                                                    comparer);
                }

                return innerDictionary;
            }
        }

        public virtual ICollection<TKey> Keys
        {
            get
            {
                return ReadDictionary.Keys;
            }
        }

        public virtual ICollection<TValue> Values
        {
            get
            {
                return ReadDictionary.Values;
            }
        }

        public virtual int Count
        {
            get
            {
                return ReadDictionary.Count;
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public virtual TValue this[TKey key]
        {
            get
            {
                return ReadDictionary[key];
            }
            set
            {
                WriteDictionary[key] = value;
            }
        }

        public virtual bool ContainsKey(TKey key)
        {
            return ReadDictionary.ContainsKey(key);
        }

        public virtual void Add(TKey key, TValue value)
        {
            WriteDictionary.Add(key, value);
        }

        public virtual bool Remove(TKey key)
        {
            return WriteDictionary.Remove(key);
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            return ReadDictionary.TryGetValue(key, out value);
        }

        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            WriteDictionary.Add(item);
        }

        public virtual void Clear()
        {
            WriteDictionary.Clear();
        }

        public virtual bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ReadDictionary.Contains(item);
        }

        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ReadDictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return WriteDictionary.Remove(item);
        }

        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ReadDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}