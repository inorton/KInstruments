using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonFxRPCServer;

namespace KInstrumentsService
{
    public class KRPCService : IKRPCService
    {
        public KService Service { get; set; }

        public InstrumentData PollData()
        {
            return Service.GetData();
        }

        public void SetThrottle(SettableCommand values)
        {
            if (Service.ControlInput != null)
            {
                Service.ControlInput.Throttle = values.Set;
            }
        }

        public void ToggleGear()
        {
            if (Service.ControlInput != null)
            {
                Service.ControlInput.ToggleGear();
            }
        }

        public void ToggleStage()
        {
            if (Service.ControlInput != null)
            {
                Service.ControlInput.ToggleStage();
            }
        }

        public void SetTrim(SettableCommand values)
        {
            if (Service.ControlInput != null)
            {
                Service.ControlInput.TrimPitch = values.Set;
            }
        }
    }
}
