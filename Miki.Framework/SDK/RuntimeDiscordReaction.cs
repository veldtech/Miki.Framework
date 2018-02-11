using Discord;

namespace Miki.Common
{
    internal class RuntimeDiscordReaction
    {
        private IReaction sourceReaction;

        public RuntimeDiscordReaction(IReaction reaction)
        {
            sourceReaction = reaction;
        }
    }
}