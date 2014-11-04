using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using KInstrumentsService;

namespace kinstruments
{
    public class KinstrumentsWebserver
    {
        static object lck = new object();
        static KinstrumentsWebserver instance = null;
        public static KinstrumentsWebserver GetInstance()
        {
            lock (lck)
            {
                if (instance == null)
                {
                    int port = 8881;
                    instance = new KinstrumentsWebserver(port, !KService.IsMono);
                    var sep = System.IO.Path.DirectorySeparatorChar.ToString();
                    var wd = System.IO.Path.Combine(KSPUtil.ApplicationRootPath, 
                        "GameData/"+
                        "Kinstruments/" +
                        "Plugins/" + 
                        "www");

                    var tmp = wd.Split('/');
                    wd = string.Join(sep, tmp);

                    instance.Service.WwwRootDirectory = wd;
                    instance.Service.Log.Print("http port {0}", port);
                    instance.Service.Log.Print("wwwrootdir = {0}", instance.Service.WwwRootDirectory);
                }
                return instance;
            }
        }


        public KinstrumentsWebserver(int port) : this( port, false )
        {
        }

        bool started = false;

        public KService Service { get; private set; }
        public KinstrumentsWebserver(int port, bool localhost)
        {
            Service = new KService(port);
        }

        public void Start()
        {
            if (!started)
            {
                lock (Service)
                {
                    Service.Start();
                    started = true;
                    Service.Log.Print("webserver started");
                }
            }
            else
            {
                Service.Log.Print("webserver already started");
            }
        }

        public void Stop()
        {
            Service.Stop();
        }
    }
}
