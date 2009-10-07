using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;

namespace Unity.AutoRegistration
{
    public class Registration : IFluentRegistration
    {
        private Type _type;

        private Func<Type, IEnumerable<Type>> _interfacesToRegisterAsResolver = t => new List<Type>(t.GetInterfaces());
        private Func<Type, string> _nameToRegisterWithResolver = t => String.Empty;
        private Func<Type, LifetimeManager> _lifetimeManagerToRegisterWithResolver = t => new TransientLifetimeManager();

        public LifetimeManager LifetimeManagerToRegisterWith
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

        public string NameToRegisterWith
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

        public IEnumerable<Type> InterfacesToRegisterAs
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

        public Type Type
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                _type = value;
            }
        }

        public IFluentRegistration UsingLifetime<TLifetimeManager>() where TLifetimeManager : LifetimeManager, new()
        {
            _lifetimeManagerToRegisterWithResolver = t => new TLifetimeManager();
            return this;
        }

        public IFluentRegistration UsingLifetime(Func<Type, LifetimeManager> lifetimeResolver)
        {
            _lifetimeManagerToRegisterWithResolver = lifetimeResolver;
            return this;
        }

        public IFluentRegistration UsingLifetime<TLifetimeManager>(TLifetimeManager manager) where TLifetimeManager : LifetimeManager
        {
            _lifetimeManagerToRegisterWithResolver = t => manager;
            return this;
        }

        public IFluentRegistration UsingSingetonMode()
        {
            _lifetimeManagerToRegisterWithResolver = t => new ContainerControlledLifetimeManager();
            return this;
        }

        public IFluentRegistration UsingPerCallMode()
        {
            _lifetimeManagerToRegisterWithResolver = t => new TransientLifetimeManager();
            return this;
        }

        public IFluentRegistration WithName(string name)
        {
            NameToRegisterWith = name;
            return this;
        }

        public IFluentRegistration WithName(Func<Type, string> nameCreator)
        {
            _nameToRegisterWithResolver = nameCreator;
            return this;
        }

        public IFluentRegistration WithTypeName()
        {
            _nameToRegisterWithResolver = t => t.Name;
            return this;
        }

        public IFluentRegistration AsInterface<TContact>() where TContact : class
        {
            _interfacesToRegisterAsResolver = t => new List<Type> { typeof(TContact) };
            return this;
        }

        public IFluentRegistration AsInterface(Func<Type, Type> interfaceResolver)
        {
            _interfacesToRegisterAsResolver = t => new List<Type> {interfaceResolver(t)};
            return this;
        }

        public IFluentRegistration AsFirstInterfaceOfType()
        {
            _interfacesToRegisterAsResolver = t => new List<Type> { t.GetInterfaces().First() };
            return this;
        }

        public IFluentRegistration AsSingleInterfaceOfType()
        {
            _interfacesToRegisterAsResolver = t => new List<Type> { t.GetInterfaces().Single() };
            return this;
        }

        public IFluentRegistration AsAllInterfacesOfType()
        {
            _interfacesToRegisterAsResolver = t => new List<Type>(t.GetInterfaces());
            return this;
        }
    }
}