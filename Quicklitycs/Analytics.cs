using System.Collections.Concurrent;

namespace Quicklitycs
{
    public class Analytics
    {
        private static Lazy<Analytics>? _instance;

        public static Analytics Instance =>
            _instance?.Value
            ?? throw new InvalidOperationException("Analytics has not been initialized.");

        private readonly IAnalyticsManager _manager;
        private readonly ConcurrentQueue<Func<Task>> _actionQueue;
        private readonly CancellationTokenSource _cts;
        private readonly Task _processingTask;

        public static void Initialize(IAnalyticsManager manager)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("Analytics has already been initialized.");
            }
            _instance = new Lazy<Analytics>(() => new Analytics(manager));
        }

        private Analytics(IAnalyticsManager manager)
        {
            _manager = manager;
            _actionQueue = new ConcurrentQueue<Func<Task>>();
            _cts = new CancellationTokenSource();
            _processingTask = Task.Run(ProcessQueueAsync);
        }

        public void Track(string eventName, IDictionary<string, object>? properties = null)
        {
            _actionQueue.Enqueue(() => _manager.TrackAsync(eventName, properties));
        }

        public void Identify(string uniqueId)
        {
            _actionQueue.Enqueue(() => _manager.IdentifyAsync(uniqueId));
        }

        public void Screen(string screenName)
        {
            _actionQueue.Enqueue(() => _manager.ScreenAsync(screenName));
        }

        private async Task ProcessQueueAsync()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                if (_actionQueue.TryDequeue(out Func<Task>? action))
                {
                    try
                    {
                        await action();
                    }
                    catch
                    {
                        _actionQueue.Enqueue(action);
                    }
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }
    }
}
