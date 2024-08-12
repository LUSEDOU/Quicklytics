namespace Quicklitycs
{
    /// <summary>
    /// Manages a single analytics provider.
    /// </summary>
    public class SingleProviderManager : BaseAnalyticsManager
    {
        /// <summary>
        /// The analytics provider.
        /// </summary>
        private readonly IAnalyticsProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleProviderManager"/> class.
        /// </summary>
        /// <param name="provider">The analytics provider.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="provider"/> is <see langword="null"/>.
        /// </exception>
        public SingleProviderManager(IAnalyticsProvider provider)
        {
            ArgumentNullException.ThrowIfNull(provider, nameof(provider));
            _provider = provider;
        }

        public override async Task IdentifyAsync(string uniqueId)
        {
            await ExecuteProviderMethodAsync(() => _provider.IdentifyAsync(uniqueId));
        }

        public override async Task ScreenAsync(string screenName)
        {
            await ExecuteProviderMethodAsync(() => _provider.ScreenAsync(screenName));
        }

        public override async Task TrackAsync(
            string eventName,
            IDictionary<string, object>? properties = null
        )
        {
            await ExecuteProviderMethodAsync(() => _provider.TrackAsync(eventName, properties));
        }
    }
}
