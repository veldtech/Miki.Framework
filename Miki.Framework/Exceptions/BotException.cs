using Miki.Framework.Languages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Exceptions
{
	public class BotException : Exception
	{
		private string _resource;
		private object[] _parameters;

		public virtual string Resource => _resource;
		public virtual object[] Parameters => _parameters;

		public static BotException CreateCustom(string resource, params object[] parameters)
		{
			return new BotException()
			{
				_resource = resource,
				_parameters = parameters
			};
		}
	}
}
