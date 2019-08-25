using System;
using System.Collections.Generic;
using System.Text;
using Miki.Localization;
using Miki.Localization.Exceptions;

namespace Miki.Framework.Commands.Permissions.Exceptions
{
    public class PermissionUnauthorizedException : LocalizedException
    {
        public override IResource LocaleResource => new LanguageResource("error_permissions_invalid");
    }
}
