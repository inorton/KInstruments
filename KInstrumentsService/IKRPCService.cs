using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JsonFxRPCServer;
using System.Runtime.Serialization;

namespace KInstrumentsService
{
    [DataContract]
    public class SettableCommand {
        [DataMember]
        public float Set { get; set; }
    }

    public interface IKRPCService 
    {
        [JsonRPCMethod]
        InstrumentData PollData();

        [JsonRPCMethod]
        void SetThrottle(SettableCommand values);

        [JsonRPCMethod]
        void SetTrim(SettableCommand values);

        [JsonRPCMethod]
        void ToggleGear();

        [JsonRPCMethod]
        void ToggleStage();
    }
}
