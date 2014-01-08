using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using JsonFx.Json;
using System.Reflection;
using System.Runtime.Serialization;
using JsonFx.Serialization.Resolvers;

namespace JsonFxRPCServer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JsonRPCMethodAttribute : Attribute
    {
    }

    public class ServerProxy
    {
        Dictionary<string, Func<object[], object>> methodHandlers = new Dictionary<string, Func<object[], object>>();
        Dictionary<string, MethodInfo> methodinfos = new Dictionary<string, MethodInfo>();
        
        public void SetMethodHandler(string name, Func<object[], object> m)
        {
            methodHandlers[name] = m;
        }

        public void AddHandlers<T>( T service )
        {
            var t = typeof(T);
            var ml = t.GetMethods();
            foreach (var m in ml)
            {
                var attrs = m.GetCustomAttributes(typeof(JsonRPCMethodAttribute), true);
                if (attrs != null)
                {
                    if (attrs.Length > 0)
                    {
                        SetMethodHandler(m.Name, 
                            (x) => { return m.Invoke(service, x); });
                        methodinfos[m.Name] = m;
                    }    
                }
            }
        }

        public void WriteResult(RPCResult res, TextWriter w)
        {
            var jw = new JsonWriter();
            jw.Write(res, w);
        }

        public RPCResult RunMethod(RPCMethod m)
        {
            var rv = new RPCResult();
            rv.id = m.Id;
            Func<object[], object> act;

            if (methodHandlers.TryGetValue(m.Name, out act))
            {
                try
                {
                    MethodInfo mi;
                    if (methodinfos.TryGetValue(m.Name, out mi))
                    {
                        var pars = mi.GetParameters();
                        if (pars.Length == m.Args.Length)
                        {
                            for (int i = 0; i < pars.Length; i++)
                            {
                                m.Args[i] = m.ConvertArg(pars[i].ParameterType, m.Args[i]);
                            }
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("incorrect number of arguments");
                        }
                    }

                    rv.result = act.Invoke(m.Args);
                }
                catch (Exception e)
                {
                    var err = new Dictionary<string, object> {
                        { "message" , string.Format("method {0} threw exception", m.Name) },
                        { "exception" , e.ToString() },
                    };
                    rv.error = err;
                }
            }
            else
            {
                var err = new Dictionary<string, object> {
                    { "message" , "no such method" },
                    { "method" , m.Name },
                };
                rv.error = err;
            }
            return rv;
        }

        public Dictionary<string, object> ReadJson(TextReader r)
        {
            var rdr = new JsonReader();
            var obj = rdr.Read<Dictionary<string, object>>(r);
            return obj;
        }

        static T CoaxTo<T>(object o)
        {
            T rv = default(T);
            if (o is T)
            {
                rv = (T)o;
            }
            return rv;
        }


        public RPCMethod ReadMethod(string m)
        {
            using (var tr = new StringReader(m))
            {
                return ReadMethod(tr);
            }
        }

        public RPCMethod ReadMethod(TextReader r)
        {
            var rv = new RPCMethod();
            var o = ReadJson(r);
            if (o != null)
            {
                if (o.ContainsKey("method"))
                {
                    rv.Name = CoaxTo<string>(o["method"]);
                }
                if (o.ContainsKey("id"))
                {
                    rv.Id = CoaxTo<int>(o["id"]); 
                }
                if (o.ContainsKey("params"))
                {
                    if ( o["params"] is Array ) {
                        var arr = o["params"] as Array;
                        rv.Args = new object[arr.Length];
                        Array.Copy(arr, rv.Args, rv.Args.Length);                        
                    }
                }
            }
            return rv;
        }
    }

    public class RPCMethod
    {
        public string Name { get; set; }
        public object[] Args { get; set; }
        public int Id { get; set; }

        public RPCMethod ()
        {
            Args = new object[0];
        }

        public T ConvertArg<T>( object o)
        {
            return (T) ConvertArg(typeof(T), o);
        }

        public object ConvertArg(Type t, object o)
        {
            var js = new JsonWriter();
            var str = js.Write(o);
            var jr = new JsonReader();
            return jr.Read(str, t);
        }
    }

    [DataContract]
    public class RPCResult
    {
        public int id { get; set; }

        public object result { get; set; }

        public Dictionary<string, object> error { get; set; }
    }
}
