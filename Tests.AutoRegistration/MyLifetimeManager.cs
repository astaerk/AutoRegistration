using System;
using Unity.Lifetime;

namespace Tests.AutoRegistration
{
    internal class MyLifetimeManager : LifetimeManager
    {
        public override object GetValue(ILifetimeContainer container = null)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            throw new NotImplementedException();
        }

        public override void RemoveValue(ILifetimeContainer container = null)
        {
            throw new NotImplementedException();
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            throw new NotImplementedException();
        }
    }
}