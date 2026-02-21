# Transcriber

A .NET 10 console app to play with the Google Cloud Speech-to-Text API. Originally created to solve the problem of my then girlfriend who was happy about sharing funny voice memos from her Brazilian cousin, and my own Portuguese is muito problemático. Now updated with a vibe coded Spectre.Console UI which was fun.

# Pre-requisites
- .NET 10 SDK
- A Google Cloud account with the Speech-to-Text API enabled

# Google Cloud Setup
Undetailed steps to point you in the right direction to set up Google Cloud for this app:

1. Create a Google Cloud Platform project
2. Enable the [Speech-to-Text API](https://docs.cloud.google.com/speech-to-text/docs) and the [Cloud Translation API](https://docs.cloud.google.com/translate/docs)
3. Create a [service account](https://docs.cloud.google.com/iam/docs/service-account-overview)
4. Give the service account a role with permissions to access the API:s, e.g. `Cloud Translation API User`.
5. In Google Cloud Console go to `IAM and admin` > `Service accounts` > `Keys` tab, click `add key` and choose JSON to download the Google credentials.

# App Configuration

- According to .NET convention the app uses `appsettings.json` for configuration, this file is checked in with some default values.
- You can add your own environment-specific appsettings file (e.g., `appsettings.Development.json`).
  - The environment-specific file will override defaults from `appsettings.json`.
  - The app will automatically load `appsettings.{ENVIRONMENT}.json` if the environment variable is set.
- Update your settings file with:
  - Path to the JSON with Google credentials (`GoogleCredentialsFilePath`)
  - Path to the data folder containing the audio files to translate (`AudioDirectoryPath`)

## Setting the Environment Name

To run the app with a specific environment (e.g., Development), set the `DOTNET_ENVIRONMENT` variable:

```powershell
# Current session
$env:DOTNET_ENVIRONMENT = "Development"

# User (persistent)
setx DOTNET_ENVIRONMENT "Development"

# Machine (persistent, requires admin)
setx DOTNET_ENVIRONMENT "Development" /M
```

# Running the App

Run the app using:

```shell
dotnet run
