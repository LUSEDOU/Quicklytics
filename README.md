# Quicklytics

Quicklytics is a library that allows you to easily integrate multiple
analytics providers into your application.

## Goals
* Use a single API to integrate multiple analytics providers.
* Agnostic to the type of application (Web, Mobile, Desktop).
* Independence of the Analytics Provider.
* Easy to use and configure.


## Packages
```
Quicklytics
|-- Quicklytics
|-- Quicklytics.Web

Quicklytics (Providers)
|-- Quicklytics.Localytics

Quicklytics (External Providers)
|-- Quicklytics.GoogleAnalytics
|-- Quicklytics.MixPanel
|-- etc.
```


### Quicklytics
This is the main package that contains the core of the library.
Is used to create your own Providers.

Supported features:
* Track (Event, Properties)
* Identify (UniqueId) \[Save the user id in the provider\]
* PageView/Screens (Name)

Future features planned:
* (Un)SetProperties (Properties)
* Time (Event)

Example
* Use

```cs
readonly IAnalyticsProvider _provider;

public async Task Login(string userId)
{
    User user;
    try {
        user = await _authService.Login(userId);

        _provider.Identify(user.Id);
    } catch (Exception ex) {
        // No recommended to track errors, instead is preffered to use a Crashlytics provider
        _provider.Track("LoginError", new { Message = ex.Message });
    }
}
```


### Quicklytics.Web
Use this package to integrate the library into your WebApp.

Inspired by the Azure Identity library, it uses the `AddAnalyticsProvider` and
`AddMultipleAnalyticsProviders` methods to integrate the providers.


Example of a single Provider

```cs
builder.Services
    .AddAnalyticsProvider<GoogleAnalyticsProvider>(options =>
    {
        options.TrackingId = "UA-XXXXX-Y";
    });
```

Multiple Providers

```cs
builder.Services
    .AddMultipleAnalyticsProviders(options =>
    {
        options.Add<GoogleAnalyticsProvider>(provider =>
        {
            provider.TrackingId = "UA-XXXXX-Y";
        });

        options.Add<GoogleTagManagerProvider>(provider =>
        {
            provider.ContainerId = "GTM-XXXXX";
        });
    });
```

Example of using Configuration
```json
{
    ...
    "GoogleAnalytics": {
        "TrackingId": "UA-XXXXX-Y"
    },
    "Mixpanel": {
        "Token": "XXXXX"
    }
}
```

```cs
builder.Services
    .AddMultipleAnalyticsProviders(options =>
    {
        // It will use the configuration section "GoogleAnalytics"
        options.Add<GoogleAnalyticsProvider>();
        // The same result but declaring the configuration
        // It will use the configuration section "Mixpanel"
        options.Add<MixpanelProvider>(builder.Configuration.GetSection("Mixpanel"));
    });
```

Also add support to `Screen` events for Razor Pages and MVC.

Razor

* Implicitly
```cs
[AnalyticsPage] // or [AnalyticsPage("MyPage")]
public class MyPage : PageModel
{
    ...
}
```

* Explicitly
```cs
public class MyPage : PageModel
{
    ...
    [AnalyticsPage("MyPage")]
    public void OnGet()
    {
        ...
    }

    [AnalyticsPage("MyPage", "POST")]
    public void OnPost()
    {
        ...
    }
}
```

MVC

* Implicitly
  It almost impossible to use it implicitly, because the action name is not
    the same as the page name.

* Explicitly
```cs
public class MyController : Controller
{
    ...
    [AnalyticsPage("MyPage")]
    public IActionResult Index()
    {
        ...
    }

    [AnalyticsPage("AnotherPage", "POST")]
    [HttpPost]
    public IActionResult AnotherPage()
    {
        ...
    }
}
```


### Quicklytics.Localytics

This package is an example of a Provider that uses the core library to integrate
with a DataBase Provider to store the events.

By default, it uses SQLite. But you can create your own implementation.
or use another database provider.

Support three modes:
#### LogTable
Write the events in a single column as `{datetime} {userId} {event} ?{properties(json)}`

Used to log the events in a log file or a single column in a database.
Support change the separator. (` `, `,`, `|`, `;`, `    `). The other ones may
cause problems encoding but you can use it.

#### SingleTable
All the records are transformed to fullfill this schema:
```sql
CREATE TABLE Events (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT,
    Event TEXT,
    Properties TEXT,
    Timestamp TEXT -- ISO8601
);
```

With the options of:
* Change the name of the table.
* Change the name of the columns.
* Add Timezone support. (Just a new column with the timezone in format `+HH:MM`)

The following methods are transformed as:
* Track (Event, Properties)
    Append the UserId? and the properties as a JSON string.
* Identify (UniqueId)
    Save as "$Identify" event, with the UniqueId as properties, and the UserId
    as null.
* PageView/Screens (Name)
    Save as "$Screen" event, with the Name and method as properties, and the appended UserId?

#### MultipleTables

Create a table for each event. The table name is the event name.
These are the schemas:

```sql
-- Events
CREATE TABLE Events (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT,
    Properties TEXT,
    Timestamp TEXT -- ISO8601
);

-- Identified Users
CREATE TABLE Users (
    UserId TEXT PRIMARY KEY,
    Properties TEXT
);

-- Relation table between anonymous and identified users
CREATE Table Identify (
    UserId TEXT PRIMARY KEY,
    UniqueId TEXT UNIQUE
);
```

Notes
* The $Identify and $Screen events are saved in the Events table.
* The $Identify event is saved with the UniqueId as properties.

Options

* IdentifyProcess (Planned for the future, only `DoNothing` is supported)
    1. Do nothing (default)
    2. When user is identified, update all the events with the `AnonymousId` to the `UserId` (Not recommended, but is the fastest way to identify the user)
    3. When event is added, check if the `UniqueId` is in the `Identify` table
       and save the event with the `UserId` instead of the `UniqueId`. (This is the
         most expensive operation, but the most accurate)
* Timezone
    Add a new column with the timezone in format `+HH:MM`
* Tables
    Change the name of the tables.
* Columns
    Change the name of the columns.

Example of Integration
* SingleTable
```cs
builder.Services
    .AddAnalyticsProvider<LocalyticsProvider>(options =>
    {
        // By default, it uses SQLite
        options.ConnectionString = "Data Source=localytics.db";
        options.Mode = LocalyticsMode.SingleTable;
    });
```

* LogTable
```cs
builder.Services
    .AddAnalyticsProvider<LocalyticsProvider>(options =>
    {
        options.LogTableOptions = new LogTableOptions
        {
            Separator = "|",
            DateFormat = "yyyy-MM-dd HH:mm:ss",
            Path = "/var/log/localytics/",
            FileGenerator = (date) => $"{date:yyyy-MM-dd}.log"
        };
        options.Mode = LocalyticsMode.LogTable;
    });
```

* MultipleTables
```cs
builder.Services
    .AddAnalyticsProvider<LocalyticsProvider>(options =>
    {
        options.MultipleTablesOptions = new MultipleTablesOptions
        {
            IdentifyProcess = IdentifyProcess.DoNothing,
            Timezone = true,
            Tables = new TablesOptions
            {
                Events = "Events",
                Users = "Users",
                Identify = "Identify"
            },
            Columns = new ColumnsOptions
            {
                UserId = "UserId",
                Event = "Event",
                Properties = "Properties",
                Timestamp = "Timestamp"
            },
            // Example with PostgreSQL
            // You can use wathever database provider you want,
            DatabaseProvider = (options) => {
                return new NpgsqlConnection(options.ConnectionString);
                // or
                // return new EDBConnection(options.ConnectionString);
            }
        };
        options.Mode = LocalyticsMode.MultipleTables;
    });
```

### External Providers

The external providers are the ones that are not included in the core library.
These are the ones that are planned to be created in the future.

* Quicklytics.GoogleAnalytics
* Quicklytics.MixPanel
