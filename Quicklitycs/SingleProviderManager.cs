namespace Quicklitycs;

public class SingleProviderManager : IAnalyticsManager
{
    private readonly IAnalyticsProvider _provider;

    public SingleProviderManager(IAnalyticsProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider, nameof(provider));
        _provider = provider;
    }

    Task IAnalyticsManager.IdentifyAsync(string uniqueId)
    {
        throw new NotImplementedException();
    }

    Task IAnalyticsManager.ScreenAsync(string screenName)
    {
        throw new NotImplementedException();
    }

    Task IAnalyticsManager.TrackAsync(string eventName, IDictionary<string, object>? properties)
    {
        throw new NotImplementedException();
    }

    private async Task ExecuteProviderMethodAsync(Func<Task> method)
    {
        try
        {
            await method();
        }
        catch (Exception ex)
        {
            throw new AnalyticsProviderException(_provider, "An error occurred while executing the analytics provider method.", ex);
        }
    }
}
