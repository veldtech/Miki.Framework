using System;

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
