namespace ServiceA.Services;


public interface IHelperService
{
    Task<string?> GetFromServiceB(Guid id);

    Task<string> PingServiceB();
}