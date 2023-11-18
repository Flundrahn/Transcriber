using Microsoft.Extensions.Configuration;

namespace Transcriber;

internal static class AppSettings
{
    private static readonly IConfigurationRoot _config = Initialize();
    internal static string GoogleCredentialsFilePath => _config["ApiCredentialsPath"] ?? throw new Exception("ApiCredentialsPath not found in appsettings.json");
    internal static string AudioFilePath => _config["DataPath"] ?? throw new Exception("AudioFilePath not found in appsettings.json");

    private static IConfigurationRoot Initialize()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }
}