namespace Server.Services;

public interface ITelnetApiService
{
    Task<string> ExecuteCommandAsync(string deviceName, int commandIndex, string[] parameterValues);
}