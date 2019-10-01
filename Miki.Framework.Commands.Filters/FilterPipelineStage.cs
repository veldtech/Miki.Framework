using Miki.Discord.Common;
using Miki.Framework.Commands.Filters;
using Miki.Framework.Commands.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Filters
{
	public interface IFilter
	{
		ValueTask<bool> CheckAsync(IContext e);
	}

	public class FilterPipelineStage : IPipelineStage
	{
		private readonly IEnumerable<IFilter> _filters;

		public FilterPipelineStage(IEnumerable<IFilter> filters)
		{
			_filters = filters;
		}

		public T GetFilterOfType<T>()
			where T : class, IFilter
		{
			if(_filters == null
				|| !_filters.Any())
			{
				return default;
			}
			return _filters.Where(x => x is T)
				.Select(x => x as T)
				.FirstOrDefault();
		}

		public async ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, Func<ValueTask> next)
		{
			foreach(var f in _filters)
			{
				if(!await f.CheckAsync(e))
				{
					return;
				}
			}
			await next();
		}
	}
}

namespace Miki.Framework.Commands
{
	public static class Extensions
	{
		public static CommandPipelineBuilder UseFilter(this CommandPipelineBuilder b, IFilter f)
			=> b.UseFilters(f);
		public static CommandPipelineBuilder UseFilters(this CommandPipelineBuilder b, params IFilter[] filters)
			=> b.UseStage(new FilterPipelineStage(filters));
	}
}
