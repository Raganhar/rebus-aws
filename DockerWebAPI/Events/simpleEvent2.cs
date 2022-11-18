using Rebus.Handlers;

namespace DockerWebAPI.Events;

public class Step2Handler : IHandleMessages<Step2Event>
{
    private readonly ILogger<Step2Handler> _logger;

    public Step2Handler(ILogger<Step2Handler> logger)
    {
        _logger = logger;
    }
    public async Task Handle(Step2Event currentDateTime)
    {
        _logger.LogInformation("Step2", currentDateTime.Counter);
        await Task.Delay(5000);
        _logger.LogInformation("Step2 finished", currentDateTime.Counter);
    }
}

public class Step2Event
{
    public int Counter { get; set; }
}