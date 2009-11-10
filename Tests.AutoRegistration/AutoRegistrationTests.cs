using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Contract;
using Unity.AutoRegistration;
using Moq;

namespace Tests.AutoRegistration
{
    [TestClass]
    public class AutoRegistrationTests
    {
        private Mock<IUnityContainer> _containerMock;
        private List<RegisterEvent> _registered;
        private readonly Predicate<Assembly> _testAssemblies = a => a.GetName().FullName.StartsWith("Tests.");
        private IUnityContainer _container;
        private delegate void RegistrationCallback(Type from, Type to, string name, LifetimeManager lifetime, InjectionMember[] ims);
        private IUnityContainer _realContainer;

        private const string KnownExternalAssembly = "Microsoft.Practices.Unity.Interception";

        [TestInitialize]
        public void SetUp()
        {
            _realContainer = new UnityContainer();

            _containerMock = new Mock<IUnityContainer>();
            _registered = new List<RegisterEvent>();
            var setup = _containerMock
                .Setup(c => c.RegisterType(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<LifetimeManager>()));
            var callback = new RegistrationCallback((from, to, name, lifetime, ips) =>
                {
                    _registered.Add(new RegisterEvent(from, to, name, lifetime));
                    _realContainer.RegisterType(from, to, name, lifetime);
                });
            
            // Using reflection, because current version of Moq doesn't support callbacks with more than 4 arguments
            setup
                .GetType()
                .GetMethod("SetCallbackWithArguments", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(setup, new object[] {callback});

            _container = _containerMock.Object;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenContainerIsNullThrowsException()
        {
            _container = null;
            _container
                .ConfigureAutoRegistration();
        }

        [TestMethod]
        public void WhenApplingAutoRegistrationWithoutAnyRulesNothingIsRegistred()
        {
            _container
                .ConfigureAutoRegistration()
                .ApplyAutoRegistration();
            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        public void WhenApplingAutoRegistrationWithOnlyAssemblyRulesNothingIsRegistred()
        {
            _container
                .ConfigureAutoRegistration()
                .IncludeAssemblies(_testAssemblies)
                .ApplyAutoRegistration();
            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        public void WhenApplingAutoRegistrationWithoutAssemblyRulesNothingIsRegistred()
        {
            _container
                .ConfigureAutoRegistration()
                .Include(If.Is<TestCache>, Then.Register())
                .ApplyAutoRegistration();

            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        public void WhenApplyMethodIsNotCalledAutoRegistrationDoesNotHappen()
        {
            _container
                .ConfigureAutoRegistration()
                .IncludeAssemblies(_testAssemblies)
                .Include(If.Is<TestCache>, Then.Register());

            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        public void WhenAssemblyIsExcludedAutoRegistrationDoesNotHappenForItsTypes()
        {
            _container
                .ConfigureAutoRegistration()
                .IncludeAssemblies(_testAssemblies)
                .Include(If.Is<TestCache>, Then.Register())
                .ExcludeAssemblies(If.ContainsType<TestCache>)
                .ApplyAutoRegistration();

            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        public void WhenExternalAssemblyIsLoadedButNotIncludedAutoRegistrationDoesNotHappenForItsTypes()
        {
            _container
                .ConfigureAutoRegistration()
                .LoadAssemblyFrom(String.Format("{0}.dll", KnownExternalAssembly))
                .Include(If.Any, Then.Register())
                .ApplyAutoRegistration();

            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        public void WhenExternalAssemblyIsLoadedAndIncludedAutoRegistrationHappensForItsTypes()
        {
            _container
                .ConfigureAutoRegistration()
                .LoadAssemblyFrom(String.Format("{0}.dll", KnownExternalAssembly))
                .IncludeAssemblies(a => a.GetName().FullName.Contains(KnownExternalAssembly))
                .Include(If.Any, Then.Register())
                .ApplyAutoRegistration();

            Assert.IsTrue(_registered.Any());
        }

        [TestMethod]
        public void WhenTypeIsExcludedAutoRegistrationDoesNotHappenForIt()
        {
            _container
                .ConfigureAutoRegistration()
                .Exclude(If.Is<TestCache>)
                .IncludeAssemblies(_testAssemblies)
                .Include(If.Is<TestCache>, Then.Register())
                .ApplyAutoRegistration();

            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        public void WhenRegisterWithDefaultOptionsTypeMustBeRegisteredAsAllInterfacesItImplementsUsingPerCallLifetimeWithEmptyName()
        {
            _container
                .ConfigureAutoRegistration()
                .IncludeAssemblies(_testAssemblies)
                .Include(If.Is<TestCache>, Then.Register())
                .ApplyAutoRegistration();

            Assert.IsTrue(_registered.Count == 2);

            var iCacheRegisterEvent = _registered.SingleOrDefault(r => r.From == typeof(ICache));
            var iDisposableRegisterEvent = _registered.SingleOrDefault(r => r.From == typeof(IDisposable));

            Assert.IsNotNull(iCacheRegisterEvent);
            Assert.IsNotNull(iDisposableRegisterEvent);
            Assert.AreEqual(typeof(TestCache), iCacheRegisterEvent.To);
            Assert.AreEqual(typeof(TransientLifetimeManager), iCacheRegisterEvent.Lifetime.GetType());
            Assert.AreEqual(String.Empty, iCacheRegisterEvent.Name);
            Assert.AreEqual(typeof(TestCache), iDisposableRegisterEvent.To);
            Assert.AreEqual(typeof(TransientLifetimeManager), iDisposableRegisterEvent.Lifetime.GetType());
            Assert.AreEqual(String.Empty, iDisposableRegisterEvent.Name);
        }

        [TestMethod]
        public void WhenRegistrationObjectIsPassedRequestedTypeRegisteredAsExpected()
        {
            const string registrationName = "TestName";
            
            var registration = Then.Register();
            registration.Interfaces = new[] {typeof(ICache)};
            registration.LifetimeManager = new ContainerControlledLifetimeManager();
            registration.Name = registrationName;

            _container
                .ConfigureAutoRegistration()
                .IncludeAssemblies(_testAssemblies)
                .Include(If.Is<TestCache>, registration)
                .ApplyAutoRegistration();

            Assert.AreEqual(1, _registered.Count);
            var registerEvent = _registered.Single();
            Assert.AreEqual(typeof(TestCache), registerEvent.To);
            Assert.AreEqual(typeof(ContainerControlledLifetimeManager), registerEvent.Lifetime.GetType());
            Assert.AreEqual(registrationName, registerEvent.Name);
        }

        [TestMethod]
        public void WhenHaveMoreThanOneRegistrationRulesTypesRegisteredAsExpected()
        {
            _container
                .ConfigureAutoRegistration()
                .IncludeAssemblies(_testAssemblies)
                .Include(If.Implements<ICustomerRepository>,
                         Then.Register()
                             .AsSingleInterfaceOfType()
                             .WithTypeName()
                             .UsingPerThreadMode())
                .Include(If.DecoratedWith<LoggerAttribute>, Then.Register().AsAllInterfacesOfType())
                .ApplyAutoRegistration();

            // 2 types implement ICustomerRepository, LoggerAttribute decorated type implement 2 interfaces
            Assert.AreEqual(4, _registered.Count);
        }

        [TestMethod]
        public void ImplementsITypeName()
        {
            Assert.IsTrue(typeof(CustomerRepository).ImplementsITypeName());
            Assert.IsTrue(typeof(Introduction).ImplementsITypeName());
        }

        [TestMethod]
        public void WhenImplementsOpenGenericTypesRegisteredAsExpected()
        {
            _container
                .ConfigureAutoRegistration()
                .IncludeAssemblies(_testAssemblies)
                .Include(type => type.ImplementsOpenGeneric(typeof(IHandlerFor<>)), 
                    Then.Register().AsFirstInterfaceOfType().WithTypeName())
                .ApplyAutoRegistration();

            Assert.AreEqual(2, _registered.Count);
            Assert.IsTrue(_registered
                .Select(r => r.To)
                .SequenceEqual(new[] { typeof(DomainEventHandlerOne), typeof(DomainEventHandlerTwo) }));
            Assert.IsTrue(_registered
                .Select(r => r.From)
                .All(t => t == typeof(IHandlerFor<DomainEvent>)));

            Assert.AreEqual(2, _realContainer.ResolveAll(typeof(IHandlerFor<DomainEvent>)).Count());
        }

        [TestMethod]
        public void WithPartName()
        {
            Assert.AreEqual(
                "Customer",
                new RegistrationOptions {Type = typeof (CustomerRepository)}
                    .WithPartName(WellKnownAppParts.Repository)
                    .Name);

            Assert.AreEqual(
                "Test",
                new RegistrationOptions { Type = typeof(TestCache) }
                    .WithPartName("Cache")
                    .Name);
        }

        private class RegisterEvent
        {
            public Type From { get; private set; }
            public Type To { get; private set; }
            public string Name { get; private set; }
            public LifetimeManager Lifetime { get; private set; }

            public RegisterEvent(Type from, Type to, string name, LifetimeManager lifetime)
            {
                From = from;
                To = to;
                Name = name;
                Lifetime = lifetime;
            }
        }

        public class Introduction : IIntroduction
        {
            
        }

        public interface IIntroduction
        {
        }

        private void Example()
        {
            var container = new UnityContainer();

            container
                .ConfigureAutoRegistration()
                .LoadAssemblyFrom("Plugin.dll")
                .IncludeAllLoadedAssemblies()
                .ExcludeSystemAssemblies()
                .ExcludeAssemblies(a => a.GetName().FullName.Contains("Test"))
                .Include(If.Implements<ILogger>, Then.Register().UsingPerCallMode())
                .Include(If.ImplementsITypeName, Then.Register().WithTypeName())
                .Include(If.Implements<ICustomerRepository>, Then.Register().WithName("Sample"))
                .Include(If.Implements<IOrderRepository>,
                         Then.Register().AsSingleInterfaceOfType().UsingPerCallMode())
                .Include(If.DecoratedWith<LoggerAttribute>,
                         Then.Register()
                             .AsInterface<IDisposable>()
                             .WithPartName(WellKnownAppParts.Logger)
                             .UsingLifetime<MyLifetimeManager>())
                .Exclude(t => t.Name.Contains("Trace"))
                .ApplyAutoRegistration();

            container
                .ConfigureAutoRegistration()
                .IncludeAllLoadedAssemblies()
                .Include(If.Implements<IView>, Then.Register().WithPartName(WellKnownAppParts.View))
                .Include(If.Implements<IController>, Then.Register().WithPartName(WellKnownAppParts.Controller))
                .ApplyAutoRegistration();
        }
    }
}