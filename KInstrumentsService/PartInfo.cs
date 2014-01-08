using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace KInstrumentsService
{
    [DataContract]
    public class FuelInfo {

        [DataMember]
        public string FuelType { get; set; }

        [DataMember]
        public int Capacity { get; set; }

        [DataMember]
        public int Contents { get; set; }
    }

    [DataContract]
    public class EngineInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string[] FuelTypes { get; set; }
    }

    [DataContract]
    public class PartInfo
    {
        [DataMember]
        public string Id { get; set; }

        public int Mass { get; set; }

        [DataMember]
        public FuelInfo[] Fuel { get; set; }

        [DataMember]
        public EngineInfo Engine { get; set; }

        [DataMember]
        public string[] Connections { get; set; }
    }
}
