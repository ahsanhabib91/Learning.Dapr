using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using ServiceA.Services;

namespace ServiceA.Controllers;

[ApiController]
[Route("[controller]")]
public class ServiceAController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<ServiceAController> _logger;
    private readonly IHelperService _helperService;
    private readonly DaprClient _daprClient;
    private readonly HttpClient _serviceB;

    // private const string storeName = "statestore";
    private const string StoreName = "weather-state-store";

    private const string PubSubName = "weather-pub-sub";
    private const string TopicName = "weather";

    public ServiceAController(ILogger<ServiceAController> logger, IHelperService helperService, DaprClient daprClient)
    {
        _logger = logger;
        _helperService = helperService;
        _daprClient = daprClient;
        _serviceB = DaprClient.CreateInvokeHttpClient("serviceB");
    }

    [HttpGet("ping")]
    public string Ping()
    {
        return "PONG from ServiceA";
    }

    private async Task<T?> Get<T>(string key, string stateStoreName = "statestore")
    {
        try
        {
            var value = await _daprClient.GetStateAsync<T>(stateStoreName, key);
            return value;
        }
        catch (Exception e)
        {
            return default(T);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<WeatherForecast?> Get(Guid id)
    {
        var weather = await Get<WeatherForecast>($"weather-{id}", StoreName);
        return weather;
    }

    [HttpGet("serviceB/{id:guid}")]
    public async Task<string?> GetFromServiceB(Guid id)
    {
        return await _helperService.GetFromServiceB(id);
    }

    [HttpGet("serviceB/ping")]
    public async Task<string> PingServiceB()
    {
        return await _helperService.PingServiceB();
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
        _logger.LogInformation($"Service: ServiceA, Store: {StoreName}");
        await _daprClient.SaveStateAsync(StoreName, key, weather);
        return weather;
    }

    [HttpDelete("{id:guid}")]
    public async Task Delete(Guid id)
    {
        var key = $"weather-{id}";
        await _daprClient.DeleteStateAsync(StoreName, key);
    }

    [HttpPost("publish/1")]
    public async Task<WeatherForecast> Save1(WeatherForecast weatherForecast)
    {
        await _daprClient.PublishEventAsync(PubSubName, TopicName, weatherForecast);
        return weatherForecast;
    }

    [HttpPost("publish/2")]
    public async Task<WeatherForecast> Save2(WeatherForecast weatherForecast)
    {
        await _daprClient.PublishEventAsync(PubSubName, "weather2", weatherForecast);
        return weatherForecast;
    }

    [HttpPost("publish/3")]
    public async Task<WeatherForecast> Save3(WeatherForecast weatherForecast)
    {
        await _daprClient.PublishEventAsync(PubSubName, "weather3", weatherForecast);
        return weatherForecast;
    }


    // [HttpGet("serviceB/{id:guid}")]
    // public async Task<WeatherForecast?> GetFromServiceB(Guid id)
    // {
    //     return await _serviceB.GetFromJsonAsync<WeatherForecast>($"weatherForecast/{id}");
    // }
}