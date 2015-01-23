using System;
using System.Runtime.Serialization;

namespace TeamodoroClient.Json
{
    [DataContract]
    public class StateOption
    {
        [DataMember(Name = "duration")]
        public long Duration { get; set; }

        [DataMember(Name = "color")]
        public String Color { get; set; }

    }
}
