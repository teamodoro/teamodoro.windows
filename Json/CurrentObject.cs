using System;
using System.Runtime.Serialization;

namespace TeamodoroClient.Json
{
    [DataContract]
    public class CurrentObject
    {
        [DataMember(Name = "name")]
        public String Name { get; set; }

        [DataMember(Name = "options")]
        public Options Options { get; set; }

        [DataMember(Name = "state")]
        public CurrentState State { get; set; }

        [DataMember(Name = "timesBeforeLongBreak")]
        public long TimesBeforeLongBreak { get; set; }

        [DataMember(Name = "currentTime")]
        public long CurrentTime { get; set; }

    }
}
