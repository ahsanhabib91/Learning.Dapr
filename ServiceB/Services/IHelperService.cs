namespace ServiceB.Services;

public interface IHelperService
{
    Task<string?> GetFromServiceA(Guid id);
    
    Task<string> PingServiceA();
}