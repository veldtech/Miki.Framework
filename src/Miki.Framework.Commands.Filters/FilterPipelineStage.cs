namespace Miki.Framework.Commands.Filters
{
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class FilterPipelineStage : IPipelineStage
	{
		private readonly IEnumerable<IFilter> filters;

		public FilterPipelineStage(IEnumerable<IFilter> filters)
		{
			this.filters = filters;
		}

		public T GetFilterOfType<T>()
			where T : class, IFilter
		{
			if(filters == null || !filters.Any())
			{
				return default;
			}
			return filters.OfType<T>().FirstOrDefault();
		}

		public async ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, Func<ValueTask> next)
		{
			foreach(var f in filters)
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
    using Miki.Framework.Commands.Filters;

    public static class Extensions
	{
		public static CommandPipelineBuilder UseFilter(this CommandPipelineBuilder b, IFilter f)
			=> b.UseFilters(f);
		public static CommandPipelineBuilder UseFilters(this CommandPipelineBuilder b, params IFilter[] filters)
			=> b.UseStage(new FilterPipelineStage(filters));
	}
}
