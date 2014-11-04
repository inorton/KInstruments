using System;

namespace KInstrumentsService
{
    public interface ILogger
    {
        void Print(string fmt, params object[] args);
    }

    public class UnityLogger : ILogger
    {
        public void Print(string fmt, params object[] args)
        {
            try
            {
                UnityEngine.MonoBehaviour.print("kinstruments: " + string.Format(fmt, args) + "\n");
            }
            catch { }
        }
    }

    public class BCLLogger : ILogger
    {
        public void Print(string fmt, params object[] args)
        {
            Console.Error.WriteLine(fmt, args);
        }
    }
}
