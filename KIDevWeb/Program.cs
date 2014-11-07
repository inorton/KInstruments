using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using kinstruments;
using KInstrumentsService;

namespace KIDevWeb
{
    public class FakeControlInput : IControlInput
    {
        public KinstrumentsWebserver Service { get; set; }

        public void CheckVessel(object vesselObject)
        {
            
        }

        public void Fly(FlightCtrlState fstate)
        {

        }

        public void DeployGear()
        {

        }

        public void StowGear()
        {

        }

        public void ToggleGear()
        {
            Console.Error.WriteLine("ToggleGear");
        }

        public void ToggleStage()
        {
            Console.Error.WriteLine("ToggleStage");
        }

        private float trimPitch;

        public float TrimPitch
        {
            get { return trimPitch; }
            set {
                Console.Error.WriteLine("pitch trimmed {0}", value);
                trimPitch = value;
            }
        }

        public bool GearDown
        {
            get;
            set;
        }

        private float throttle;

        public float Throttle
        {
            get { return throttle; }
            set {
                Console.Error.WriteLine("throttle set {0}", value);
                throttle = value;
                Service.Service.GetData().Throttle = value;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var s = new KinstrumentsWebserver(8881, true);
            s.Service.Log = new KInstrumentsService.BCLLogger();
            var fci = new FakeControlInput();
            fci.Service = s;
            s.Service.ControlInput = fci;
            s.Service.WwwRootDirectory = System.IO.Path.Combine(Environment.CurrentDirectory, "www");
            s.Start();

            InstrumentData d = s.Service.GetData();

            string k = string.Empty;
            do
            {
                d.Throttle = s.Service.ControlInput.Throttle;
                d.GearDown = s.Service.ControlInput.GearDown;
                d.VesselName = "Test Vessel";
                k = Console.ReadKey(true).KeyChar.ToString();

                switch (k)
                {
                    case "q":
                        d.Roll -= 2;
                        break;
                    case "e":
                        d.Roll += 2;
                        break;
                    case "w":
                        d.Pitch -= 4;
                        d.Altitude -= 100;
                        break;
                    case "s":
                        d.Pitch += 4;
                        d.Altitude += 100;
                        break;

                    case "z":
                        d.SurfaceVelocity -= 12;
                        break;

                    case "x":
                        d.SurfaceVelocity += 24;
                        break;
                }

            } while (true);
        }
    }
}
