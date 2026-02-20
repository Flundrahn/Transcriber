namespace Transcriber.Animations;

public class Spinner : IDisposable
{
    private readonly Thread _thread;
    private bool _isRunning;
    private const int FrameLengthMilliSeconds = 180;
    private readonly static char[] _spinnerChars = ['|', '/', '-', '\\'];

    public Spinner()
    {
        _isRunning = true;
        _thread = new Thread(Spin);
        _thread.Start();
    }

    private void Spin()
    {
        int index = 0;
        while (_isRunning)
        {
            Console.Write(_spinnerChars[index]);
            Thread.Sleep(FrameLengthMilliSeconds);
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            index = (index + 1) % _spinnerChars.Length;
        }
    }

    public void Dispose()
    {
        _isRunning = false;
        _thread.Join();
    }
}