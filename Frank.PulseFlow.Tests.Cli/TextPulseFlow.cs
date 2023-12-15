namespace Frank.PulseFlow.Tests.Cli;

public class TextPulseFlow : IFlow
{
    private readonly ILogger<TextPulseFlow> _logger;

    public TextPulseFlow(ILogger<TextPulseFlow> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(IPulse message, CancellationToken cancellationToken)
    {
        if (message is TextPulse textMessage)
            _logger.LogInformation("Received text message: {Text}", textMessage.Text);
        await Task.CompletedTask;
    }

    public bool CanHandle(Type pulseType) => pulseType == typeof(TextPulse);
}