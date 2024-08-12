namespace Quicklitycs
{
    public interface IAnalytics
    {
        void Track(string eventName, IDictionary<string, object>? properties = null);

        void Identify(string uniqueId);

        void Screen(string screenName);
    }

}
