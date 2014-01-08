using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XR.Server.Http;

using XR.Include;
using System.IO;
using JsonFxRPCServer;

namespace KInstrumentsService
{
 
    public class KService
    {        
        public static bool IsMono
        {
            get
            {
                return Type.GetType("Mono.Runtime") != null;
            }
        }

        public IControlInput ControlInput { get; set; }

        public IInstrumentDataSource InstrumentDataSource { get; set; }

        InstrumentData idata;

        public DateTime LastInstrumentUpdate { get; private set; }


        InstrumentData InstrumentData
        {
            get
            {
                return idata;
            }
            set
            {
                LastInstrumentUpdate = DateTime.Now;
                idata = value;
            }
        }

        ServerProxy jsonrpc;
        public KRPCService Service { get; private set; }

        public string WwwRootDirectory { get; set; }
        public ILogger Log { get; set; }
        HttpServer WebServer { get; set; }

        

        public KService( int port )
        {
            Log = new UnityLogger();
            WebServer = new HttpServer();
            jsonrpc = new ServerProxy();
            Service = new KRPCService();
            if (!IsMono)
            {
                WebServer.Localhostonly = true;
            }

            jsonrpc.AddHandlers((IKRPCService)Service);
            Service.Service = this;

            WebServer.Port = port;

            WebServer.UriRequested += WebServer_UriRequested;
            WebServer.UriRequested += WebServer_Json;
            WebServer.UriRequested += WebServer_FileServer;
            WebServer.UriRequested += WebServer_FileIndex;
            WebServer.UriRequested += WebServer_FileNotFound;

            pageModels.Add(new KIWebContext(this) { PagePath = "/index.html", Title = "KInstruments Home" });
            pageModels.Add(new KIWebContext(this) { PagePath = "/radalt.html", Title = "Altitude (RADAR)" });
            pageModels.Add(new KIWebContext(this) { PagePath = "/analogalt.html", Title = "Altitude (MSL)" }); 
            pageModels.Add(new KIWebContext(this) { PagePath = "/navball.html", Title = "Nav Ball" });
            pageModels.Add(new KIWebContext(this) { PagePath = "/hframe_nav_rad.html", Title = "Nav Ball + Radar Alt" });
            pageModels.Add(new KIWebContext(this) { PagePath = "/gear_stage.html", Title = "Gear / Stage" });
        }


        void WebServer_UriRequested(object sender, UriRequestEventArgs args)
        {
            Log.Print("requested {0}", args.Request.Url);
        }

        public void Stop()
        {
            WebServer.StopServer();
        }

        void WebServer_Json(object sender, UriRequestEventArgs args)
        {
            if (!args.Handled)
            {
                if (args.Request.Url.AbsolutePath != "/json/") return;
                args.Handled = true;
                try
                {
                    
                    var reqtype = args.Request.ContentType;
                    var reqenc = args.Request.ContentEncoding;
                    var len = args.Request.ContentLength64;

                    var sr = new StreamReader(args.Request.InputStream);

                    var m = jsonrpc.ReadMethod(sr);
                    var r = jsonrpc.RunMethod(m);
                    args.SetResponseState(200);
                    args.SetResponseType("application/json");
                    jsonrpc.WriteResult(r, args.ResponsStream);
                   
                }
                catch (Exception e)
                {
                    Log.Print("exception!: {0}", e);
                    args.SetResponseState(500);
                    args.SetResponseType("text/plain");
                    args.ResponsStream.WriteLine(e.ToString());
                }
            }
        }

        void WebServer_FileNotFound(object sender, UriRequestEventArgs args)
        {
            if (!args.Handled)
            {
                args.SetResponseState(404);
                args.SetResponseType("text/plain");
                args.Handled = true;
                args.ResponsStream.WriteLine("not found : {0}", args.Request.Url);
            }
        }

        Dictionary<string, string> ctypes = new Dictionary<string, string>()
        {
            { ".txt", "text/plain" },
            { ".html", "text/html" },
            { ".htm", "text/html" },
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".jpeg", "image/jpeg" },
            { ".css", "text/css" },
            { ".js", "text/javascript" },
        };

        List<KIWebContext> pageModels = new List<KIWebContext>();

        public string GetContentType(string filename)
        {
            var ext = System.IO.Path.GetExtension(filename).ToLower();
            var rv = "application/x-octet-strem";
            ctypes.TryGetValue(ext, out rv);
            return rv;
        }

        KIWebContext GetPageModel(string abspath)
        {
            abspath = abspath.Replace("//", "/");
            KIWebContext rv = (from x in pageModels where x.PagePath == abspath select x).FirstOrDefault();
            return rv;
        }

        Processor GetProcessor()
        {
            var proc = new MethodProcessor();
            if (WwwRootDirectory == null)
            {
                proc.RootDirectory = this.GetType().Assembly.Location;
            }
            else
            {
                proc.RootDirectory = WwwRootDirectory;
            }
            return proc;
        }

        void WebServer_FileServer(object sender, UriRequestEventArgs args)
        {
            if (!args.Handled)
            {
                var path = args.Request.Url.AbsolutePath;
                var proc = GetProcessor();
                var localpath = proc.VirtualToLocalPath(path);
                if (Directory.Exists(localpath))
                {
                    var indexpage = Path.Combine(localpath,"index.html");
                    if (File.Exists(indexpage))
                        localpath = indexpage;
                    path = string.Join("/", new string[] { path, "index.html" });
                }

                if (File.Exists(localpath))
                {
                    args.SetResponseState(200);                    
                    args.SetResponseType(GetContentType(localpath));
                    args.Handled = true;

                    if ( path.StartsWith("/static/") )
                    {
                        using ( var fh = File.OpenRead(localpath) )
                        {
                            var buf = new byte[8192];
                            int count = 0;
                            do {
                                count = fh.Read( buf, 0, buf.Length );
                                if ( count > 0 ) args.ResponsStream.BaseStream.Write( buf, 0, count );
                                if (count < buf.Length) break;
                            } while ( true );
                        }
                    } else {
                        proc.Context = GetPageModel(path);
                        proc.Transform(path, args.ResponsStream);
                    }
                }

            }
        }

        void WebServer_FileIndex(object sender, UriRequestEventArgs args)
        {
            if (!args.Handled)
            {
                var path = args.Request.Url.AbsolutePath;
                if (path == "/" || path.StartsWith("/static/"))
                {
                    var proc = GetProcessor();
                    args.SetResponseState(200);
                    args.SetResponseType("text/html");

                    var dirs = System.IO.Directory.GetDirectories(proc.VirtualToLocalPath(args.Request.Url.AbsolutePath));
                    var files = System.IO.Directory.GetFiles(proc.VirtualToLocalPath(args.Request.Url.AbsolutePath));
                    args.ResponsStream.WriteLine("<html><head><title>Index of {0}</title></head>", args.Request.Url.AbsolutePath);
                    args.ResponsStream.WriteLine("<body><h1>Index Of {0}</h1><hr><ul>", args.Request.Url.AbsolutePath);

                    foreach (var d in dirs)
                    {
                        args.ResponsStream.WriteLine("<li>[DIR] <a href='{0}'>{0}</a></li>", d);
                    }

                    foreach (var f in files)
                    {
                        var fn = System.IO.Path.GetFileName(f);
                        args.ResponsStream.WriteLine("<li><a href='{0}'>{0}</a></li>", fn);
                    }

                    args.ResponsStream.WriteLine("</ul></body></html>");
                    args.Handled = true;
                }
            }
        }

        public void Start()
        {
            WebServer.BeginListen();
        }

        
        public void OnUpdate(Vessel v)
        {
            lock (WebServer)
            {
                if (idata != null)
                    idata.UpdateFromVessel(v);
                LastInstrumentUpdate = DateTime.Now;
            }
        }

        public virtual InstrumentData GetData()
        {
            lock (WebServer)
            {
                if (idata == null) idata = new InstrumentData();
                if (InstrumentDataSource != null)
                {
                    if ( DateTime.Now.Subtract(LastInstrumentUpdate).TotalMilliseconds > 200 )
                        idata = InstrumentDataSource.GetData();
                }
                return idata;
            }
        }
    }
}
