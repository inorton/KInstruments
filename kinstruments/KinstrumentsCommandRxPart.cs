using kinstruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KInstrumentsService;

namespace kinstruments
{
    public class KinstrumentsCommandRxPart : PartModule
    {
        static KinstrumentsWebserver httpd;

        public override void OnFixedUpdate()
        {
            httpd.Service.ControlInput.CheckVessel(vessel);
            base.OnFixedUpdate();
        }

        public override void OnStart(PartModule.StartState state)
        {
            if (httpd == null)
            {
                httpd = KinstrumentsWebserver.GetInstance();
            }
            base.OnStart(state);        
        }

        public override void OnActive()
        {
            base.OnActive();
            print("KinstrumentsCommandRxPart active..");
        }
    }
}

