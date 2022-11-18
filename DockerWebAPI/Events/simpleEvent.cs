using Rebus.Handlers;

namespace DockerWebAPI.Events;

public class Step1Handler : IHandleMessages<Step1Event>
{
    private readonly ILogger<Step1Handler> _logger;

    public Step1Handler(ILogger<Step1Handler> logger)
    {
        _logger = logger;
    }
    public async Task Handle(Step1Event currentDateTime)
    {
        _logger.LogInformation("Step1", currentDateTime.DateTime, currentDateTime.Counter);
        // _logger.LogInformation("The time is {0} for counter {1}", currentDateTime.DateTime, currentDateTime.Counter);
        await Task.Delay(5000);
        _logger.LogInformation("Step1 finished", currentDateTime.DateTime, currentDateTime.Counter);
        // _logger.LogInformation("Finished with {0} for counter {1}", currentDateTime.DateTime, currentDateTime.Counter);
    }
}

public class Step1Event
{
    public DateTime DateTime { get; set; }
    public int Counter { get; set; }
}