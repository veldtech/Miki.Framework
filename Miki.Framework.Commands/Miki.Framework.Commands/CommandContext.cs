using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Commands
{
	public class CommandContext
	{
		private Dictionary<string, object> _allContextObjects = new Dictionary<string, object>();

		public T Get<T>(string key)
			where T : class
		{
			_allContextObjects.TryGetValue(key, out object value);
			return value as T;
		}

		public void Set<T>(string key, T value)
			where T : class
		{
			Type type = typeof(T);

			if (_allContextObjects.ContainsKey(key))
			{
				_allContextObjects[key] = value;
			}
			else
			{
				_allContextObjects.Add(key, value);
			}
		}
	}
}
