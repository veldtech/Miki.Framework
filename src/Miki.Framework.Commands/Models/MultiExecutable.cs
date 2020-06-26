using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Stages
{
    public class MultiExecutable : IExecutable
    {
        private readonly IReadOnlyList<NodeResult> results;

        public MultiExecutable(IReadOnlyList<NodeResult> results)
        {
            this.results = results;
        }

        public async ValueTask ExecuteAsync(IContext request)
        {
            var pack = request.GetArgumentPack().Pack;
            
            foreach (var result in results)
            {
                try
                {
                    pack.SetCursor(result.CursorPosition);
                    await ((IExecutable)result.Node).ExecuteAsync(request);
                    return;
                }
                catch (MissingArgumentException)
                {
                    // ignore.
                }
            }

            throw new MissingArgumentException();
        }
    }
}