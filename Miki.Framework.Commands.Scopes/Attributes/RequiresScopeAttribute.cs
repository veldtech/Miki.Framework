namespace Miki.Framework.Commands.Scopes.Attributes
{
    using System;

    public class RequiresScopeAttribute : Attribute
	{
		public string ScopeId { get; }

		public RequiresScopeAttribute(string scopeId)
		{
			ScopeId = scopeId.ToLowerInvariant();
		}
	}
}
