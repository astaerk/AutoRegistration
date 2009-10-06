using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;

namespace Unity.AutoRegistration
{
    public class Registration : IFluentRegistration
    {
        private Type _type;

        private Func<Type, ICollection<Type>> _interfacesToRegisterAsResolver = t => new List<Type>();
        private Func<Type, string> _nameToRegisterWithResolver = t => String.Empty;

        public Registration()
        {
            LifetimeManagerToRegisterWith = new TransientLifetimeManager();
        }

        public LifetimeManager LifetimeManagerToRegisterWith { get; set; }

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

        public ICollection<Type> InterfacesToRegisterAs
        {
            get
            {
                return _interfacesToRegisterAsResolver(_type);
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
            LifetimeManagerToRegisterWith = new TLifetimeManager();
            return this;
        }

        public IFluentRegistration UsingLifetime<TLifetimeManager>(TLifetimeManager manager) where TLifetimeManager : LifetimeManager
        {
            LifetimeManagerToRegisterWith = manager;
            return this;
        }

        public IFluentRegistration UsingSingetonMode()
        {
            LifetimeManagerToRegisterWith = new ContainerControlledLifetimeManager();
            return this;
        }

        public IFluentRegistration UsingPerCallMode()
        {
            LifetimeManagerToRegisterWith = new TransientLifetimeManager();
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