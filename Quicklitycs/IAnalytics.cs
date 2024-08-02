namespace Quicklitycs
{
    public interface IAnalytics
    {
        public void Identify(string userId);

        public void Track(string eventName, Dictionary<string, object> properties);

        public void Screen(string screenName, Dictionary<string, object> properties);
    }
}
