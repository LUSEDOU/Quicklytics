namespace Quicklitycs
{
    public class IAnalyticsInstance
    {
        public static IAnalyticsInstance Instance => Nested.Instance;

        private class Nested
        {
            static Nested() { }

            internal static readonly IAnalyticsInstance Instance = new();
        }
    }
}
