using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Commands.Scopes.Attributes
{
    public class RequiresScopeAttribute : Attribute
    {
        public string ScopeId { get; }

        public RequiresScopeAttribute(string scopeId)
        {
            ScopeId = scopeId.ToLowerInvariant();
        }
    }
}
