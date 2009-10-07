using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace Unity.AutoRegistration
{
    public interface IRegistration
    {
        LifetimeManager LifetimeManagerToRegisterWith { get; set; }
        string NameToRegisterWith { get; set; }
        IEnumerable<Type> InterfacesToRegisterAs { get; set; }
        Type Type { set; }
    }
}