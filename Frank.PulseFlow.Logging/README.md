# Frank.PulseFlow.Logging

This library provides a simple logger for use in .NET applications. It uses the `Microsoft.Extensions.Logging` library 
backed by `Frank.PulseFlow` for logging. It will log to the console by default, but can add one or more `IFlow`'s to do 
whatever you want with the log messages. A common use case is to log to a file or a database, and because `Frank.PulseFlow` 
is thread-safe, you can do so without worrying about concurrency issues like file locks, or the overhead of waiting for a lock.

## Usage

```csharp
using Frank.PulseFlow.Logging;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = new HostBuilder()
        .ConfigureLogging((hostContext, logging) =>
        {
            logging.AddPulseFlow();
        })
        .ConfigureServices((hostContext, services) =>
        {
            services.AddPulseFlow<FileLoggerFlow>();
        });
        .Build();
        
        await builder.RunAsync();
    }
}

public class FileLoggerFlow(IOptions<FileLoggerSettings> options) : IFlow
{
    private readonly FileLoggerSettings _settings = options.Value;
    
    public async Task HandleAsync(IPulse pulse, CancellationToken cancellationToken)
    {
        var thing = pulse as LogPulse;
        await File.AppendAllTextAsync(_settings.LogPath, thing! + Environment.NewLine, cancellationToken);
    }

    public bool CanHandle(Type pulseType) => pulseType == typeof(LogPulse);
}

public class FileLoggerSettings
{
    public string LogPath { get; set; } = "../../../../logs.log";
}
```

## Configuration

The `AddPulseFlow` method has a few overloads that allow you to configure the logger. The default configuration is to log
to the console, but you can add one or more `IFlow`'s to the logger to do whatever you want with the log messages. A common
use case is to log to a file or a database, and because `Frank.PulseFlow` is thread-safe, you can do so without worrying
about concurrency issues like file locks, or the overhead of waiting for a lock.

## Contributing

Contributions are welcome! Please see create an issue before submitting a pull request to discuss the changes you would like to make.

## License

This library is licensed under the MIT license. See the [LICENSE](../LICENSE) file for more information.
