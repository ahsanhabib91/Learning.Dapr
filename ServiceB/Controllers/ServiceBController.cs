using System.Net;
using System.Security.Claims;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceB.Services;

namespace ServiceB.Controllers;

[ApiController]
[Route("[controller]")]
public class ServiceBController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<ServiceBController> _logger;
    private readonly IHelperService _helperService;
    private readonly DaprClient _daprClient;

    // private const string StoreName = "statestore";
    private const string StoreName = "weather-state-store";

    private const string PubSubName = "weather-pub-sub";
    private const string TopicName = "weather";

    public ServiceBController(ILogger<ServiceBController> logger, IHelperService helperService, DaprClient daprClient)
    {
        _logger = logger;
        _helperService = helperService;
        _daprClient = daprClient;
    }

    [HttpGet("ping")]
    public string Ping()
    {
        return "PONG from ServiceB";
    }

    [HttpGet("{id:guid}")]
    public async Task<WeatherForecast> Get(Guid id)
    {
        var key = $"weather-{id}";
        var weather = await _daprClient.GetStateAsync<WeatherForecast>(StoreName, key);
        return weather;
    }

    [HttpGet("serviceA/{id:guid}")]
    public async Task<string?> GetFromServiceB(Guid id)
    {
        return await _helperService.GetFromServiceA(id);
    }

    [HttpGet("serviceA/ping")]
    public async Task<string> PingServiceA()
    {
        return await _helperService.PingServiceA();
    }

    [HttpPost]
    public async Task<WeatherForecast> Save()
    {
        var id = Guid.NewGuid();
        var key = $"weather-{id}";
        var weather = new WeatherForecast
        {
            Id = id,
            Date = DateTime.Now,
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        };
        _logger.LogInformation($"Service: ServiceB, Store: {StoreName}");
        Dictionary<string, string> metadata = new Dictionary<string, string>();
        metadata.TryAdd("ttlInSeconds", "5");
        // await _daprClient.SaveStateAsync(weatherstateName, key, weather, metadata: metadata);
        await _daprClient.SaveStateAsync(StoreName, key, weather);
        return weather;
    }

    [HttpDelete("{id:guid}")]
    public async Task Delete(Guid id)
    {
        var key = $"weather-{id}";
        await _daprClient.DeleteStateAsync(StoreName, key);
    }

    [HttpPost("subscribe/1")]
    [Topic(PubSubName, TopicName)]
    [ProducesResponseType(typeof(WeatherForecast), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Subscribe1(WeatherForecast weatherForecast)
    {
        _logger.LogInformation($"1 Weather summary: {weatherForecast.Summary}");
        return Ok();
    }

    [HttpPost("subscribe/2")]
    [Topic(PubSubName, "weather2")]
    [ProducesResponseType(typeof(WeatherForecast), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Subscribe2(WeatherForecast weatherForecast)
    {
        _logger.LogInformation($"2 Weather summary: {weatherForecast.Summary}");
        return Ok();
    }

    [HttpPost("subscribe/3")]
    [Topic(PubSubName, "weather3")]
    [ProducesResponseType(typeof(WeatherForecast), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Subscribe3(WeatherForecast weatherForecast)
    {
        _logger.LogInformation($"3 Weather summary: {weatherForecast.Summary}");
        return Ok();
    }
}