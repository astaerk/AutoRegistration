using System.Diagnostics;
using Tests.Contracts;

namespace Tests.ExternalAssembly
{
    [Logger]
    public class TraceLogger : ILogger
    {
        #region ILogger Members

        public void Log(string message)
        {
            Debug.WriteLine(message);
        }

        #endregion
    }
}