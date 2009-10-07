using System;
using Microsoft.Practices.Unity;

namespace Unity.AutoRegistration
{
    /// <summary>
    /// Fluent registration options contract describes number of operations 
    /// to fluently set registration option values
    /// </summary>
    public interface IFluentRegistration : IRegistrationOptions
    {
        /// <summary>
        /// Specifies lifetime manager to use when registering type
        /// </summary>
        /// <typeparam name="TLifetimeManager">The type of the lifetime manager.</typeparam>
        /// <returns>Fluent registration</returns>
        IFluentRegistration UsingLifetime<TLifetimeManager>() where TLifetimeManager : LifetimeManager, new();

        /// <summary>
        /// Specifies lifetime manager resolver function, that by given type return lifetime manager to use when registering type
        /// </summary>
        /// <param name="lifetimeResolver">Lifetime manager resolver.</param>
        /// <returns>Fluent registration</returns>
        IFluentRegistration UsingLifetime(Func<Type, LifetimeManager> lifetimeResolver);

        /// <summary>
        /// Specifies lifetime manager to use when registering type
        /// </summary>
        /// <typeparam name="TLifetimeManager">The type of the lifetime manager.</typeparam>
        /// <returns>Fluent registration</returns>
        IFluentRegistration UsingLifetime<TLifetimeManager>(TLifetimeManager manager) where TLifetimeManager : LifetimeManager;

        /// <summary>
        /// Specifies ContainerControlledLifetimeManager lifetime manager to use when registering type
        /// </summary>
        /// <returns>Fluent registration</returns>
        IFluentRegistration UsingSingetonMode();


        /// <summary>
        /// Specifies TransientLifetimeManager lifetime manager to use when registering type
        /// </summary>
        /// <returns>Fluent registration</returns>
        IFluentRegistration UsingPerCallMode();

        /// <summary>
        /// Specifies PerThreadLifetimeManager lifetime manager to use when registering type
        /// </summary>
        /// <returns>Fluent registration</returns>
        IFluentRegistration UsingPerThreadMode();

        /// <summary>
        /// Specifies name to register type with
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>Fluent registration</returns>
        IFluentRegistration WithName(string name);

        /// <summary>
        /// Specifies name resolver function that by given type returns name to register it with
        /// </summary>
        /// <param name="nameResolver">Name resolver.</param>
        /// <returns>Fluent registration</returns>
        IFluentRegistration WithName(Func<Type, string> nameResolver);

        /// <summary>
        /// Specifies that type name should be used to register it with
        /// </summary>
        /// <returns>Fluent registration</returns>
        IFluentRegistration WithTypeName();

        /// <summary>
        /// Specifies interface to register type as
        /// </summary>
        /// <typeparam name="TContact">The type of the interface.</typeparam>
        /// <returns>Fluent registration</returns>
        IFluentRegistration AsInterface<TContact>() where TContact : class;

        /// <summary>
        /// Specifies interface resolver function that by given type returns interface register type as
        /// </summary>
        /// <param name="interfaceResolver">Interface resolver.</param>
        /// <returns>Fluent registration</returns>
        IFluentRegistration AsInterface(Func<Type, Type> interfaceResolver);

        /// <summary>
        /// Specifies interface resolver function that by given type returns interfaces register type as
        /// </summary>
        /// <param name="interfacesResolver">Interface resolver.</param>
        /// <returns>Fluent registration</returns>
        IFluentRegistration AsInterface(Func<Type, Type[]> interfacesResolver);

        /// <summary>
        /// Specifies that type should be registered as its first interface
        /// </summary>
        /// <returns>Fluent registration</returns>
        IFluentRegistration AsFirstInterfaceOfType();

        /// <summary>
        /// Specifies that type should be registered as its single interface
        /// </summary>
        /// <returns>Fluent registration</returns>
        IFluentRegistration AsSingleInterfaceOfType();

        /// <summary>
        /// Specifies that type should be registered as all its interfaces
        /// </summary>
        /// <returns>Fluent registration</returns>
        IFluentRegistration AsAllInterfacesOfType();
    }
}