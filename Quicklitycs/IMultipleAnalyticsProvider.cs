namespace Quicklitycs
{
    public interface IMultipleAnalyticsProvider : IAnalyticsProvider
    {
        public void AddProvider(IAnalyticsProvider provider);
        public void RemoveProvider(IAnalyticsProvider provider);
    }
}
