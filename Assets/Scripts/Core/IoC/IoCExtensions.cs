namespace Core.IoC
{
    public static class SimpleIoCExtensions
    {
        public static void ResolveAll(this IIoC container, params object[] items)
        {
            for (int i = 0, count = items.Length; i < count; i += 1) {
                container.ResolveAll(items[i]);
            }
        }
    }
}