using System;
using Microsoft.Practices.Unity;

namespace Tests.AutoRegistration
{
    internal class MyLifetimeManager : LifetimeManager
    {
        public override object GetValue()
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object newValue)
        {
            throw new NotImplementedException();
        }

        public override void RemoveValue()
        {
            throw new NotImplementedException();
        }
    }
}