using Microsoft.Extensions.Configuration;

namespace Transcriber;

internal static class AppSettings
{
    private static readonly IConfigurationRoot _config;

    static AppSettings()
    {
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
    }

    internal static string GoogleCredentialsFilePath => _config["ApiCredentialsPath"] ?? throw new Exception("ApiCredentialsPath not found in appsettings.json");
    internal static string AudioFilePath => _config["DataPath"] ?? throw new Exception("AudioFilePath not found in appsettings.json");
}