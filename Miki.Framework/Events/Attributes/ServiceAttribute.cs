using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Events.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ServiceAttribute : Attribute
    {
		public string Name { get; internal set; }

		public ServiceAttribute(string name)
		{
			Name = name;
		}
	}
}
