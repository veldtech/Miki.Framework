using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Exceptions
{
    class ArgObjectNullException : BotException
    {
		public override string Resource => "error_argument_null";
		public override object[] Parameters => new object[] { "[docs](https://github.com/Mikibot/Miki/wiki)" };
	}
}
