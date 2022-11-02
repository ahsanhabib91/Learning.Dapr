namespace ServiceB.Services;

public class HelperService : IHelperService
{
    private readonly HttpClient _client;

    public HelperService(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<string?> GetFromServiceA(Guid id)
    {
        var response = await _client.GetAsync($"serviceA/{id}");
        var dataAsString = await response.Content.ReadAsStringAsync();
        return dataAsString;
    }
    
    public async Task<string> PingServiceA()
    {
        var response = await _client.GetAsync("serviceA/ping");
        var dataAsString = await response.Content.ReadAsStringAsync();
        return dataAsString;
    }
}