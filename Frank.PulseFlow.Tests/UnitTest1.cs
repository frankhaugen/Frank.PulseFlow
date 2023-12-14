using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Frank.PulseFlow.Tests;

public class PulseFlowTests
{
    [Fact]
    public void Test1()
    {
        var host = CreateHostBuilder().Build();
        var pulseFlow = host.Services.GetRequiredService<IConduit>();
        
        
        
    }
    
    private class MyClass : BasePulse
    {
        public string Text { get; set; }
    }
    
    private IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddPulseFlow(builder =>
                {
                    // builder.AddFlow<Flow1>();
                    // builder.AddFlow<Flow2>();
                });
            });
    }
}