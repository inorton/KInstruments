using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace KInstrumentsService
{
    [DataContract]
    public class InstrumentData
    {
        public InstrumentData()
        {
            VesselName = "n/a";
        }

        [DataMember]
        public string VesselName { get; set; }
        [DataMember]
        public double Heading { get; set; }
        [DataMember]
        public double Pitch { get; set; }
        [DataMember]
        public double Roll { get; set; }
        [DataMember]
        public double Altitude { get; set; }
        [DataMember]
        public double TerrainAltitude { get; set; }
        [DataMember]
        public double SurfaceVelocity { get; set; }
        [DataMember]
        public double OrbitalVelocity { get; set; }
        [DataMember]
        public float Throttle { get; set; }
        [DataMember]
        public int CurrentStage { get; set; }

        [DataMember]
        public bool GearDown { get; set; }

        public void UpdateFromVessel(Vessel vessel)
        {
            VesselName = vessel.vesselName;
            var vesselMiddle = vessel.findWorldCenterOfMass();
            var up = (vesselMiddle - vessel.mainBody.position).normalized;
            var north = Vector3d.Exclude(up, (vessel.mainBody.position + vessel.mainBody.transform.up * (float)vessel.mainBody.Radius) - vesselMiddle).normalized;

            var rotSurface = Quaternion.LookRotation(north, up);
            var rotVesselToSurface = Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(vessel.GetTransform().rotation) * rotSurface);

            var heading = rotVesselToSurface.eulerAngles.y;
            var pitch = (rotVesselToSurface.eulerAngles.x > 180) ? (360.0 - rotVesselToSurface.eulerAngles.x) : -rotVesselToSurface.eulerAngles.x;
            var roll = (rotVesselToSurface.eulerAngles.z > 180) ? (rotVesselToSurface.eulerAngles.z - 360.0) : rotVesselToSurface.eulerAngles.z;

            Heading = heading;
            Pitch = pitch;
            Roll = roll;
            Altitude = vessel.altitude;
            TerrainAltitude = vessel.terrainAltitude;

            CurrentStage = vessel.currentStage;
            SurfaceVelocity = vessel.rb_velocity.magnitude;

            GearDown = FlightInputHandler.state.gearDown;
            Throttle = FlightInputHandler.state.mainThrottle;
        }
    }
}
