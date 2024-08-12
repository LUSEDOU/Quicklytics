namespace Quicklitycs
{
    public interface IAnalyticsManager
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

    public abstract class BaseAnalyticsManager : IAnalyticsManager
    {
        /// <summary>
        /// The unsent actions that need to be sent.
        /// </summary>
        private Queue<Func<Task>> _unsentActions = new();

        public abstract Task IdentifyAsync(string uniqueId);

        public abstract Task ScreenAsync(string screenName);

        public abstract Task TrackAsync(
            string eventName,
            IDictionary<string, object>? properties = null
        );

        private static async Task<Queue<Func<Task>>> ExcecuteUnsentActionsAsync(
            Queue<Func<Task>> actions
        )
        {
            Queue<Func<Task>> unsentActions = new();

            foreach (Func<Task> action in actions)
            {
                try
                {
                    await action();
                }
                catch
                {
                    unsentActions.Enqueue(action);
                }
            }
            return unsentActions;
        }

        /// <summary>
        /// Queues an action to be sent.
        /// </summary>
        protected async Task ExecuteProviderMethodAsync(Func<Task> method)
        {
            _unsentActions = await ExcecuteUnsentActionsAsync(_unsentActions);
            try
            {
                await method();
            }
            catch
            {
                _unsentActions.Enqueue(method);
            }
        }
    }
}
