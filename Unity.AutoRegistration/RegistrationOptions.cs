using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;

namespace Unity.AutoRegistration
{
    /// <summary>
    /// Type registration options
    /// </summary>
    public class RegistrationOptions : IFluentRegistration
    {
        private Type _type;

        private Func<Type, IEnumerable<Type>> _interfacesToRegisterAsResolver = t => new List<Type>(t.GetInterfaces());
        private Func<Type, string> _nameToRegisterWithResolver = t => String.Empty;
        private Func<Type, LifetimeManager> _lifetimeManagerToRegisterWithResolver = t => new TransientLifetimeManager();

        /// <summary>
        /// Gets or sets lifetime manager to use to register type(s).
        /// </summary>
        /// <value>Lifetime manager.</value>
        public LifetimeManager LifetimeManager
        {
            get
            {
                return _lifetimeManagerToRegisterWithResolver(_type);
            }
            set
            {
                _lifetimeManagerToRegisterWithResolver = t => value;
            }
        }

        /// <summary>
        /// Gets or sets name to register type(s) with.
        /// </summary>
        /// <value>Name.</value>
        public string Name
        {
            get
            {
                return _nameToRegisterWithResolver(_type);
            }
            set
            {
                _nameToRegisterWithResolver = t => value;
            }
        }

        /// <summary>
        /// Gets or sets interfaces to register type(s) as.
        /// </summary>
        /// <value>Interfaces.</value>
        public IEnumerable<Type> Interfaces
        {
            get
            {
                return _interfacesToRegisterAsResolver(_type);
            }
            set
            {
                _interfacesToRegisterAsResolver = t => value;
            }
        }

        /// <summary>
        /// Sets type being registered.
        /// </summary>
        /// <value>Target type.</value>
        public Type Type
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                _type = value;
            }
        }

        /// <summary>
        /// Specifies lifetime manager to use when registering type
        /// </summary>
        /// <typeparam name="TLifetimeManager">The type of the lifetime manager.</typeparam>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration UsingLifetime<TLifetimeManager>() where TLifetimeManager : LifetimeManager, new()
        {
            _lifetimeManagerToRegisterWithResolver = t => new TLifetimeManager();
            return this;
        }

        /// <summary>
        /// Specifies lifetime manager resolver function, that by given type return lifetime manager to use when registering type
        /// </summary>
        /// <param name="lifetimeResolver">Lifetime manager resolver.</param>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration UsingLifetime(Func<Type, LifetimeManager> lifetimeResolver)
        {
            if (lifetimeResolver == null)
                throw new ArgumentNullException("lifetimeResolver");

            _lifetimeManagerToRegisterWithResolver = lifetimeResolver;
            return this;
        }

        /// <summary>
        /// Specifies lifetime manager to use when registering type
        /// </summary>
        /// <typeparam name="TLifetimeManager">The type of the lifetime manager.</typeparam>
        /// <param name="manager"></param>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration UsingLifetime<TLifetimeManager>(TLifetimeManager manager) where TLifetimeManager : LifetimeManager
        {
            if (manager == null)
                throw new ArgumentNullException("manager");

            _lifetimeManagerToRegisterWithResolver = t => manager;
            return this;
        }

        /// <summary>
        /// Specifies ContainerControlledLifetimeManager lifetime manager to use when registering type
        /// </summary>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration UsingSingetonMode()
        {
            _lifetimeManagerToRegisterWithResolver = t => new ContainerControlledLifetimeManager();
            return this;
        }

        /// <summary>
        /// Specifies TransientLifetimeManager lifetime manager to use when registering type
        /// </summary>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration UsingPerCallMode()
        {
            _lifetimeManagerToRegisterWithResolver = t => new TransientLifetimeManager();
            return this;
        }

        /// <summary>
        /// Specifies PerThreadLifetimeManager lifetime manager to use when registering type
        /// </summary>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration UsingPerThreadMode()
        {
            _lifetimeManagerToRegisterWithResolver = t => new PerThreadLifetimeManager();
            return this;
        }

        /// <summary>
        /// Specifies name to register type with
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration WithName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Name = name;
            return this;
        }

        /// <summary>
        /// Specifies name resolver function that by given type returns name to register it with
        /// </summary>
        /// <param name="nameResolver">Name resolver.</param>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration WithName(Func<Type, string> nameResolver)
        {
            if (nameResolver == null)
                throw new ArgumentNullException("nameResolver");

            _nameToRegisterWithResolver = nameResolver;
            return this;
        }

        /// <summary>
        /// Specifies that type name should be used to register it with
        /// </summary>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration WithTypeName()
        {
            _nameToRegisterWithResolver = t => t.Name;
            return this;
        }

        /// <summary>
        /// Specifies that type should be registered with its name minus well-known application part name.
        /// For example: WithPartName("Controller") will register 'HomeController' type with name 'Home',
        /// or WithPartName(WellKnownAppParts.Repository) will register 'CustomerRepository' type with name 'Customer'
        /// </summary>
        /// <param name="name">Application part name.</param>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration WithPartName(string name)
        {
            _nameToRegisterWithResolver = t =>
                                              {
                                                  var typeName = t.Name;
                                                  if (typeName.EndsWith(name))
                                                      return typeName.Remove(typeName.Length - name.Length);
                                                  return typeName;
                                              };
            return this;
        }

        /// <summary>
        /// Specifies interface to register type as
        /// </summary>
        /// <typeparam name="TContact">The type of the interface.</typeparam>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration As<TContact>() where TContact : class
        {
            _interfacesToRegisterAsResolver = t => new List<Type> { typeof(TContact) };
            return this;
        }

        /// <summary>
        /// Specifies interface resolver function that by given type returns interface register type as
        /// </summary>
        /// <param name="typeResolver">Interface resolver.</param>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration As(Func<Type, Type> typeResolver)
        {
            if (typeResolver == null)
                throw new ArgumentNullException("typeResolver");

            _interfacesToRegisterAsResolver = t => new List<Type> {typeResolver(t)};
            return this;
        }

        /// <summary>
        /// Specifies interface resolver function that by given type returns interfaces register type as
        /// </summary>
        /// <param name="typesResolver">Interface resolver.</param>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration As(Func<Type, Type[]> typesResolver)
        {
            if (typesResolver == null)
                throw new ArgumentNullException("typesResolver");

            _interfacesToRegisterAsResolver = t => new List<Type> ( typesResolver(t) );
            return this;
        }

        /// <summary>
        /// Specifies that type should be registered as its first interface
        /// </summary>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration AsFirstInterfaceOfType()
        {
            _interfacesToRegisterAsResolver = t => new List<Type> { t.GetInterfaces().First() };
            return this;
        }

        /// <summary>
        /// Specifies that type should be registered as its single interface
        /// </summary>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration AsSingleInterfaceOfType()
        {
            _interfacesToRegisterAsResolver = t => new List<Type> { t.GetInterfaces().Single() };
            return this;
        }

        /// <summary>
        /// Specifies that type should be registered as all its interfaces
        /// </summary>
        /// <returns>Fluent registration</returns>
        public IFluentRegistration AsAllInterfacesOfType()
        {
            _interfacesToRegisterAsResolver = t => new List<Type>(t.GetInterfaces());
            return this;
        }
    }
}