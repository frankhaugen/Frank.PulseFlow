namespace Frank.PulseFlow.Tests.Cli;

public class TestingService : BackgroundService
{
    private readonly IConduit _messenger;

    public TestingService(IConduit messenger)
    {
        _messenger = messenger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(2000, stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            await _messenger.SendAsync(new TextPulse { Id = Guid.NewGuid(), Text = "Hello World" });
            await Task.Delay(1000, stoppingToken);
        }
    }
}