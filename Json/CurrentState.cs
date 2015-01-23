using System;
using System.Runtime.Serialization;

namespace TeamodoroClient.Json
{
    [DataContract]
    public class CurrentState
    {
        [DataMember(Name = "name")]
        public String Name { get; set; }

    }
}
