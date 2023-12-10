namespace Frank.PulseFlow.Tests.Cli;

public class TextPulseFlow(ILogger<TextPulseFlow> logger) : IPulseFlow
{
    public async Task HandleAsync(IPulse message, CancellationToken cancellationToken)
    {
        if (message is TextPulse textMessage)
        {
            logger.LogInformation("Received text message: {Text}", textMessage.Text);
        }

        await Task.CompletedTask;
    }

    public Type AppliesTo { get; } = typeof(TextPulse);
}