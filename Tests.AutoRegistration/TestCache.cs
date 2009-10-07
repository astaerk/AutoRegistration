using System;

namespace Tests.AutoRegistration
{
    public class TestCache : ICache, IDisposable
    {
        public void Set(string key, object value)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}