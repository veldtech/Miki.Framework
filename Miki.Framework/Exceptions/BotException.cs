using Miki.Framework.Languages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Exceptions
{
    public class BotException : Exception
    {
		public virtual string Resource => "error_default";

		public BotException() : base()
		{ }
    }
}
