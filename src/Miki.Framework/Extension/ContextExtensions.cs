namespace Miki.Framework
{
    public static class ContextExtensions
    {
        public static T GetContext<T>(this IContext context, string key)
        {
            var value = context.GetContext(key);
            if(value is T castValue)
            {
                return castValue;
            }
            return default;
        }

        public static void SetContext<T>(this IMutableContext context, string key, T value)
        {
            context.SetContext(key, value);
        }
    }
}
