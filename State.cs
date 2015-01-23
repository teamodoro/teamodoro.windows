using System.Diagnostics.CodeAnalysis;

namespace TeamodoroClient
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum State
    {
        running,
        paused,
        shortBreak,
        longBreak
    }
}
