using Microsoft.Extensions.Configuration;

public static class AppConfigService
{

    public static IConfigurationRoot InitAppConfigService()
    {
        var Configuration = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
        return Configuration;
    }
}