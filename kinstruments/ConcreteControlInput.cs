using KInstrumentsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace kinstruments
{
    internal class ConcreteControlInput : IControlInput
    {
        internal class FlightInputs
        {
            public bool SetTrimPitch { get; set; }
            public float TrimPitch { get; set; }
            public bool SetThrottle { get; set; } 
            public float Throttle { get; set; }
        }

        FlightInputs controlInputs = new FlightInputs();

        Vessel CurrentVessel
        {
            get
            {
                return FlightGlobals.ActiveVessel;
            }
        }

        Vessel ControlledVessel { get; set; }

        public void Fly(FlightCtrlState fstate)
        {
            if (controlInputs != null)
            {
                if (controlInputs.SetTrimPitch)
                {
                    controlInputs.SetTrimPitch = false;
                    fstate.pitchTrim = controlInputs.TrimPitch;
                }
                if (controlInputs.SetThrottle)
                {
                    controlInputs.SetThrottle = false;
                    fstate.mainThrottle = controlInputs.Throttle;
                    FlightInputHandler.state.mainThrottle = fstate.mainThrottle; // make screen match
                }
            }
        }

        public void DeployGear()
        {
            CurrentVessel.ActionGroups.SetGroup(KSPActionGroup.Gear, true);
        }

        public void StowGear()
        {
            CurrentVessel.ActionGroups.SetGroup(KSPActionGroup.Gear, false);
        }

        public void ToggleStage()
        {
            Staging.ActivateNextStage();
        }

        public void ToggleGear()
        {
            if (GearDown)
            {
                DeployGear();
            }
            else
            {
                StowGear();
            }
        }

        public bool GearDown
        {
            get { 
                return CurrentVessel.ActionGroups[KSPActionGroup.Gear];
            }
        }

        public float TrimPitch
        {
            get
            {
                return FlightInputHandler.state.pitchTrim;
            }
            set
            {
                controlInputs.TrimPitch = value;
                controlInputs.SetTrimPitch = true;
            }
        }

        public float Throttle
        {
            get
            {
                return FlightInputHandler.state.mainThrottle;
            }
            set
            {
                controlInputs.Throttle = value;
                controlInputs.SetThrottle = true;
            }
        }

        void SetControlledVessel(Vessel v)
        {
            if (v == null) return;
            if (v != ControlledVessel)
            {
                if (ControlledVessel != null)
                {
                    ControlledVessel.OnFlyByWire -= Fly;
                }
                ControlledVessel = v;
                ControlledVessel.OnFlyByWire -= Fly;
                ControlledVessel.OnFlyByWire += Fly;
            }
        }

        public void CheckVessel(object vesselObject)
        {
            if ((object)CurrentVessel == vesselObject)
            {
                if (CurrentVessel != ControlledVessel)
                {
                    SetControlledVessel(CurrentVessel);
                }
            }
        }
    }
}
