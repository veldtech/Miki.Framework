using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Exceptions
{
    public class SessionInUseException : BotException
    {
		public override string Resource => "error_session_in_use";
	}
}
