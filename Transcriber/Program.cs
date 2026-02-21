using Microsoft.Extensions.Configuration;
using Transcriber;
using Transcriber.Ui;

static void ValidateAppSettings(AppSettings appSettings)
{
    var settingsValidator = new AppSettingsValidator();
    settingsValidator.Validate(appSettings);

    foreach (string warning in settingsValidator.Warnings)
        ConsoleExtensions.WriteWarningLine(warning);

    foreach (string error in settingsValidator.Errors)
        ConsoleExtensions.WriteErrorLine(error);

    if (settingsValidator.Errors.Count > 0)
        throw new InvalidOperationException("App settings are invalid, please fix the errors and try again.");
}

string? environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
    .Build();

var appSettings = config.Get<AppSettings>()
    ?? throw new InvalidOperationException("Failed to bind AppSettings, please check the settings json and try again.");

ValidateAppSettings(appSettings);

// So Google Cloud libraries can find the credentials file path 
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", appSettings.GoogleCredentialsFilePath);

var audioFileProvider = new AudioFileProvider(appSettings);
var app = new ConsoleApp(appSettings, audioFileProvider);

await app.RunAsync();

