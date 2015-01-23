using System.Runtime.Serialization;

namespace TeamodoroClient.Json
{
    [DataContract]
    public class Options
    {
        [DataMember(Name = "running")]
        public StateOption Running { get; set; }

        [DataMember(Name = "shortBreak")]
        public StateOption ShortBreak { get; set; }

        [DataMember(Name = "longBreak")]
        public StateOption LongBreak { get; set; }

        [DataMember(Name = "longBreakEvery")]
        public long LongBreakEvery { get; set; }

        [DataMember(Name = "aliveTimeout")]
        public long AliveTimeout { get; set; }

    }
}
