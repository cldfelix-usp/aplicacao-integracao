
namespace Server;
public static class Configuration
{
    private static IConfiguration _config;

    public static void Initialize(IConfiguration configuration)
    {
        _config = configuration;
    }

    public static string GetValue(string key)
    {
        return _config[key];
    }

    public static string GetConnectionString(string name)
    {
        return _config.GetConnectionString(name);
    }

    //public static string this[string key] => GetValue(key);
}