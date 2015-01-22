using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

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
