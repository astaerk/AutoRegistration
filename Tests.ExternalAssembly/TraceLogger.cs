using System.Diagnostics;
using Tests.Contract;

namespace Tests.ExternalAssembly
{
    [Logger]
    public class TraceLogger : ILogger
    {
        #region ILogger Members

        public void Log(string message)
        {
            Trace.WriteLine(message);
        }

        #endregion
    }
}