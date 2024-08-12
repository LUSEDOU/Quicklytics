# Quicklytics (Core)

## Concepts

The concepts of the Core library are:
1. The `AnalyticsProvider` class is the one that developers will use to
   integrate their analytics provider.
2. The `AnalyticsOptions` class is the one that developers will use to configure the `AnalyticsProvider`. They have to create a class for their provider that inherits from `AnalyticsOptions`.
3. The `AnalyticsManager` are the class that will manage the provider. Now there
   is only two of them: `SingleAnalyticsManager` and `MultipleAnalyticsManager`.
4. The `AnalyticsManager` has to be Singleton, so the developer has to register it
   as a Singleton in the Dependency Injection container.
