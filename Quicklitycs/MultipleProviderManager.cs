namespace Quicklitycs;

public class MultipleProviderManager : BaseAnalyticsManager
{
    /// <summary>
    /// The analytics providers.
    /// </summary>
    private readonly IEnumerable<IAnalyticsProvider> _providers;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleProviderManager"/> class.
    /// </summary>
    /// <param name="providers">The analytics providers.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="providers"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="providers"/> is empty.
    /// </exception>
    public MultipleProviderManager(IEnumerable<IAnalyticsProvider> providers)
    {
        ArgumentNullException.ThrowIfNull(providers, nameof(providers));
        if (!providers.Any())
        {
            throw new ArgumentException("At least one provider is required.", nameof(providers));
        }
        _providers = providers;
    }

    public override async Task IdentifyAsync(string uniqueId)
    {
        await Task.WhenAll(_providers.Select(provider => provider.IdentifyAsync(uniqueId)));
    }

    public override async Task ScreenAsync(string screenName)
    {
        await Task.WhenAll(_providers.Select(provider => provider.ScreenAsync(screenName)));
    }

    public override async Task TrackAsync(
        string eventName,
        IDictionary<string, object>? properties = null
    )
    {
        await Task.WhenAll(
            _providers.Select(provider => provider.TrackAsync(eventName, properties))
        );
    }
}
