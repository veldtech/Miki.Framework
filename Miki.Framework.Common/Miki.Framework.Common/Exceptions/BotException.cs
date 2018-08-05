using System;

namespace Miki.Framework.Exceptions
{
	public class BotException : Exception
	{
		public virtual string Resource => "error_default";
		public virtual object[] Parameters => new object[] { };
    }
}
