using Miki.Localization;
using Miki.Localization.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Exceptions
{
    public class SessionInUseException : LocalizedException
    {
		public override IResource LocaleResource
			=> new LanguageResource("error_session_in_use");
	}
}
