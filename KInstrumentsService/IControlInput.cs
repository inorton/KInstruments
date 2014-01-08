using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KInstrumentsService
{

    public interface IControlInput
    {
        void CheckVessel(Object vesselObject);
        void Fly(FlightCtrlState fstate);
        void DeployGear();
        void StowGear();
        void ToggleGear();
        void ToggleStage();

        float TrimPitch { get; set; }
        bool GearDown { get; }
        float Throttle { get; set; }
    }
}
