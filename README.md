# Transcriber

A .NET 8 console app to play with the Google Cloud Speech-to-Text API.

# Setup

1. Create a Google Cloud Platform project and enable the [Speech-to-Text API](https://cloud.google.com/speech-to-text/docs/quickstart).
2. [Create a service account](https://www.youtube.com/watch?v=t5mbV3vxa-Y)
3. Ensure service account has a role with permission to use the Speech-to-Text API, seems that `Client` is sufficient.
   ![Google Cloud Project settings for adding role to service account](Screenshot.png)
4. Add a key to service account and download the JSON with google credentials.
5. Clone the repo.
6. Add a data folder with some audio files to transcribe.
7. Update `appsettings.json` with
   - path to the JSON with google credentials
   - path to the data folder.
8. Run the app using `dotnet run`
