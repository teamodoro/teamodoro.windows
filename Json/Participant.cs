using System;
using System.Runtime.Serialization;

namespace TeamodoroClient.Json
{
    [DataContract]
    class Participant
    {
        [DataMember(Name = "session")]
        public String Session { get; set; }

        [DataMember(Name = "name")]
        public String Name { get; set; }

        [DataMember(Name = "lastAccess")]
        public long LastAccess { get; set; }
    }
}
