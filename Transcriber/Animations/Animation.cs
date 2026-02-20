namespace Transcriber.Animations;

public static class Animation
{
    private static Spinner? _spinner;

    internal static void ShowSpinner()
    {
        _spinner = new Spinner();
    }


    internal static void HideSpinner()
    {
        if (_spinner is null)
        {
            return;
        }

        _spinner.Dispose();
    }
}