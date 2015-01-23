using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace TeamodoroClient
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum State
    {
        [Description("Running")]
        running,
        [Description("Paused")]
        paused,
        [Description("Short Break")]
        shortBreak,
        [Description("Long Break")]
        longBreak,
    }

}
