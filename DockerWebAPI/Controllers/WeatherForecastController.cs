using Dapper;
using DockerWebAPI.Events;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Rebus.Bus;
using Timer = System.Timers.Timer;

namespace DockerWebAPI.Controllers;

[ApiController]
[Route("")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IBus _bus;
    private static Timer _timer;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public string Get()
    {
        foreach (var i in Enumerable.Range(0,30))
        {
            _bus.SendLocal(new Step1Event
            {
                DateTime = DateTime.Now,
                Counter = i
            }).Wait(); 
            _bus.SendLocal(new Step2Event()
            {
                Counter = i
            }).Wait();   
        }
        return "hello b";
    }
}