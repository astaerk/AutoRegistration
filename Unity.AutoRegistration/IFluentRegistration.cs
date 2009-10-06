using System;
using Microsoft.Practices.Unity;

namespace Unity.AutoRegistration
{
    public interface IFluentRegistration : IRegistration
    {
        IFluentRegistration UsingLifetime<TLifetimeManager>() where TLifetimeManager : LifetimeManager, new();
        IFluentRegistration UsingLifetime<TLifetimeManager>(TLifetimeManager manager) where TLifetimeManager : LifetimeManager;
        IFluentRegistration UsingSingetonMode();
        IFluentRegistration UsingPerCallMode();

        IFluentRegistration WithName(string name);
        IFluentRegistration WithName(Func<Type, string> nameCreator);
        IFluentRegistration WithTypeName();

        IFluentRegistration AsInterface<TContact>() where TContact : class;
        IFluentRegistration AsFirstInterfaceOfType();
        IFluentRegistration AsSingleInterfaceOfType();
        IFluentRegistration AsAllInterfacesOfType();
    }
}