using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;

namespace Unity.AutoRegistration
{
    public class AutoRegistration
    {
        private readonly List<RegistrationEntry> _registrationEntries = new List<RegistrationEntry>();
        private readonly List<Predicate<Assembly>> _excludedAssemblyFilters = new List<Predicate<Assembly>>();

        private readonly List<Predicate<Assembly>> _includedAssemblyFilters = new List<Predicate<Assembly>>();

        private readonly List<Predicate<Type>> _excludedTypeFilters = new List<Predicate<Type>>();
        private readonly IUnityContainer _container;

        public AutoRegistration (IUnityContainer container)
        {
            _container = container;
        }

        public AutoRegistration Include(
            Predicate<Type> typeFilter,
            Action<Type, IUnityContainer> registrator)
        {
            _registrationEntries.Add(new RegistrationEntry(typeFilter, registrator, _container));
            return this;
        }

        public AutoRegistration Include(
            Predicate<Type> typeFilter,
            IRegistration registration)
        {
            _registrationEntries.Add(new RegistrationEntry(
                                         typeFilter,
                                         (t, c) =>
                                             {
                                                 registration.Type = t;
                                                 foreach (var contract in registration.InterfacesToRegisterAs)
                                                 {
                                                     c.RegisterType(
                                                         contract,
                                                         t,
                                                         registration.NameToRegisterWith,
                                                         registration.LifetimeManagerToRegisterWith);
                                                 }
                                             },
                                         _container));
            return this;
        }

        public AutoRegistration ExcludeAssemblies(Predicate<Assembly> filter)
        {
            _excludedAssemblyFilters.Add(filter);
            return this;
        }

        public AutoRegistration IncludeAssemblies(Predicate<Assembly> filter)
        {
            _includedAssemblyFilters.Add(filter);
            return this;
        }

        public AutoRegistration IncludeAllLoadedAssemblies()
        {
            _includedAssemblyFilters.Insert(0, a => true);
            return this;
        }

        public AutoRegistration Exclude(Predicate<Type> filter)
        {
            _excludedTypeFilters.Add(filter);
            return this;
        }

        public void ApplyAutoRegistration()
        {   
            foreach (var type in AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !_excludedAssemblyFilters.Any(f => f(a)))
                .Where(a => _includedAssemblyFilters.Any(f => f(a)))
                .SelectMany(a => a.GetTypes())
                .Where(t => !_excludedTypeFilters.Any(f => f(t))))
                foreach (var entry in _registrationEntries)
                    entry.RegisterIfSatisfiesFilter(type);
        }

        private class RegistrationEntry
        {
            private readonly Predicate<Type> _typeFilter;
            private readonly Action<Type, IUnityContainer> _registrator;
            private readonly IUnityContainer _container;

            public RegistrationEntry(Predicate<Type> typeFilter,
                                     Action<Type, IUnityContainer> registrator, IUnityContainer container)
            {
                _typeFilter = typeFilter;
                _registrator = registrator;
                _container = container;
            }

            public void RegisterIfSatisfiesFilter(Type type)
            {
                if (_typeFilter(type))
                    _registrator(type, _container);
            }
        }
    }
}