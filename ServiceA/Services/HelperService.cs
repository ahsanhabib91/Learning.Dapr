namespace ServiceA.Services;

public class HelperService : IHelperService
{
    private readonly HttpClient _client;

    public HelperService(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<string?> GetFromServiceB(Guid id)
    {
        var response = await _client.GetAsync($"serviceB/{id}");
        var dataAsString = await response.Content.ReadAsStringAsync();
        return dataAsString;
    }
    
    public async Task<string> PingServiceB()
    {
        var response = await _client.GetAsync("serviceB/ping");
        var dataAsString = await response.Content.ReadAsStringAsync();
        return dataAsString;
    }
}