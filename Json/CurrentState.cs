using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace TeamodoroClient.Json
{
    [DataContract]
    public class CurrentState
    {
        [DataMember(Name = "name")]
        public String Name { get; set; }

    }
}
