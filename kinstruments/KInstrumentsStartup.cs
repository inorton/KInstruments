using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace kinstruments
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class KInstrumentsStartup : MonoBehaviour
    {
        static object lck = new object();
        static bool started = false;

        static KinstrumentsWebserver serviceInstance = null;

        public void Start()
        {
            lock (lck)
            {
                if (!started)
                {
                    var ws = KinstrumentsWebserver.GetInstance();
                    ws.Service.Log.Print("KInstrumentsStartup");
                    ws.Service.InstrumentDataSource = new PollingInstrumentDataSource();
                    ws.Start();
                    serviceInstance = ws;
                    started = true;
                    DontDestroyOnLoad(this);
                }
            }
        }
    }
}
