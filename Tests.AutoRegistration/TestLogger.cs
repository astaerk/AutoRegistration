using System;
using Tests.Contracts;

namespace Tests.AutoRegistration
{
    [Logger]
    public class TestLogger : ILogger, IDisposable
    {
        #region ILogger Members

        public void Log(string message)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}