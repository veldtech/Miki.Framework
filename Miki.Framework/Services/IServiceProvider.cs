using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Services
{
	public interface IReadOnlyServiceProvider
	{
		T GetService<T>();
	}

	public interface IServiceProvider
	{
		void AddService<T>(T value);
	}
}
