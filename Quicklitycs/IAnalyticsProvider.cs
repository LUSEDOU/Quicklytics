namespace Quicklitycs
{
    /// <summary>
    /// Provides a way to track analytics events.
    /// </summary>
    public interface IAnalyticsProvider
    {
        /// <summary>
        /// Tracks an event, along with optional properties.
        /// </summary>
        Task TrackAsync(string eventName, IDictionary<string, object>? properties = null);

        /// <summary>
        /// Identifies a user.
        /// </summary>
        Task IdentifyAsync(string uniqueId);

        /// <summary>
        /// Tracks a screen view.
        /// </summary>
        Task ScreenAsync(string screenName);
    }

    /// <summary>
    /// Represents an instance of an analytics provider.
    /// </summary>
    public class AnalyticsProviderException(
        IAnalyticsProvider? provider,
        string message,
        Exception? innerException = null
    ) : Exception(message, innerException)
    {
        /// <summary>
        /// The analytics provider that caused the exception.
        /// </summary>
        public IAnalyticsProvider? Provider { get; } = provider;
    }
}
