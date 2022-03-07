﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Lifetime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Contracts;
using Unity.AutoRegistration;
using Moq;
using Unity.Injection;

namespace Tests.AutoRegistration
{

    [TestClass]
    public class AutoRegistrationFixture
    {
#if NET40TESTS
        private const string TESTCATEGORY = "NET40";
#else
        private const string TESTCATEGORY = "NETSTANDARD AND NET45";
#endif
        
        private Mock<IUnityContainer> _containerMock;
        private List<RegisterEvent> _registered;
        private IUnityContainer _container;
        private delegate void RegistrationCallback(Type from, Type to, string name, ITypeLifetimeManager lifetime, InjectionMember[] ims);
        private IUnityContainer _realContainer;

        [TestInitialize]
        public void SetUp()
        {
            _realContainer = new UnityContainer();
            
            _containerMock = new Mock<IUnityContainer>();
            _registered = new List<RegisterEvent>();
            var setup = _containerMock
                .Setup(c => c.RegisterType(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<ITypeLifetimeManager>()))
                .Callback<Type, Type, string, ITypeLifetimeManager, InjectionMember[]>((from, to, name, lifetime, ips) =>
                {
                    _registered.Add(new RegisterEvent(from, to, name, lifetime));
                    _realContainer.RegisterType(from, to, name, lifetime);
                });

            _container = _containerMock.Object;
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenContainerIsNull_ThrowsException()
        {
            _container = null;
            _container
                .ConfigureAutoRegistration();
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenApplingAutoRegistrationWithoutAnyRules_NothingIsRegistred()
        {
            _container
                .ConfigureAutoRegistration()
                .ApplyAutoRegistration();
            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenApplingAutoRegistrationWithOnlyAssemblyRules_NothingIsRegistred()
        {
            _container
                .ConfigureAutoRegistration()
                .ApplyAutoRegistration();
            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenApplyMethodIsNotCalled_AutoRegistrationDoesNotHappen()
        {
            _container
                .ConfigureAutoRegistration()
                .Include(If.Is<TestCache>, Then.Register());

            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenAssemblyIsExcluded_AutoRegistrationDoesNotHappenForItsTypes()
        {
            _container
                .ConfigureAutoRegistration()
                .Include(If.Is<TestCache>, Then.Register())
                .ExcludeAssemblies(If.ContainsType<TestCache>)
                .ApplyAutoRegistration();

            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenSystemAssembliesAreExcluded_AutoRegistrationDoesNotHappenForTheirTypes()
        {
            _container
                .ConfigureAutoRegistration()
                .Include(If.Is<String>, Then.Register())
                .Include(If.Is<Uri>, Then.Register())
                .ExcludeSystemAssemblies()
                .ApplyAutoRegistration();

            Assert.IsFalse(_registered.Any());
        }


        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenTypeIsExcluded_AutoRegistrationDoesNotHappenForIt()
        {
            _container
                .ConfigureAutoRegistration()
                .Exclude(If.Is<TestCache>)
                .Include(If.Is<TestCache>, Then.Register())
                .ApplyAutoRegistration();

            Assert.IsFalse(_registered.Any());
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenRegisterWithDefaultOptions_TypeMustBeRegisteredAsAllInterfacesItImplementsUsingPerCallLifetimeWithEmptyName()
        {
            _container
                .ConfigureAutoRegistration()
                .Include(If.Is<TestCache>, Then.Register())
                .ApplyAutoRegistration();

            Assert.IsTrue(_registered.Count == 2);

            var iCacheRegisterEvent = _registered.SingleOrDefault(r => r.From == typeof(ICache));
            var iDisposableRegisterEvent = _registered.SingleOrDefault(r => r.From == typeof(IDisposable));

            Assert.IsNotNull(iCacheRegisterEvent);
            Assert.IsNotNull(iDisposableRegisterEvent);
            Assert.AreEqual(typeof(TestCache), iCacheRegisterEvent.To);
            Assert.AreEqual(typeof(TransientLifetimeManager), iCacheRegisterEvent.Lifetime.GetType());
            Assert.AreEqual(null, iCacheRegisterEvent.Name);
            Assert.AreEqual(typeof(TestCache), iDisposableRegisterEvent.To);
            Assert.AreEqual(typeof(TransientLifetimeManager), iDisposableRegisterEvent.Lifetime.GetType());
            Assert.AreEqual(null, iDisposableRegisterEvent.Name);
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenRegistrationObjectIsPassed_RequestedTypeRegisteredAsExpected()
        {
            const string registrationName = "TestName";
            
            var registration = Then.Register();
            registration.Interfaces = new[] {typeof(ICache)};
            registration.LifetimeManager = new ContainerControlledLifetimeManager();
            registration.Name = registrationName;

            _container
                .ConfigureAutoRegistration()
                .Include(If.Is<TestCache>, registration)
                .ApplyAutoRegistration();

            Assert.AreEqual(1, _registered.Count);
            var registerEvent = _registered.Single();
            Assert.AreEqual(typeof(TestCache), registerEvent.To);
            Assert.AreEqual(typeof(ContainerControlledLifetimeManager), registerEvent.Lifetime.GetType());
            Assert.AreEqual(registrationName, registerEvent.Name);
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenHaveMoreThanOneRegistrationRules_TypesRegisteredAsExpected()
        {
            _container
                .ConfigureAutoRegistration()
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
        [TestCategory(TESTCATEGORY)]
        public void WhenImplementsITypeNameMehtodCalled_ItWorksAsExpected()
        {
            Assert.IsTrue(typeof(CustomerRepository).ImplementsITypeName());
            Assert.IsTrue(typeof(Introduction).ImplementsITypeName());
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenImplementsOpenGenericTypes_RegisteredAsExpected()
        {
            _container
                .ConfigureAutoRegistration()
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
        [TestCategory(TESTCATEGORY)]
        public void WhenRegistrationOfOpenGenericType_RegisteredAsExpected()
        {
            _container
                .ConfigureAutoRegistration()
                .Include(type => If.ImplementsITypeName(type) && type.Equals(typeof(Filter<>)), Then.Register())
                .ApplyAutoRegistration();

            Assert.AreEqual(1, _registered.Count);
            Assert.IsTrue(_registered
                .Select(r => r.To)
                .SequenceEqual(new[] { typeof(Filter<>) }));
            Assert.IsTrue(_registered
                .Select(r => r.From)
                .All(t => t == typeof(IFilter<>)));

            var result = _realContainer.Resolve<IFilter<string>>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void WhenWithPartNameMehtodCalled_ItWorksAsExpected()
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

        /// <summary>
        /// test to ensure, that each test platform uses the correct assembly that it should use
        /// (we already had problems with that in the past)
        /// </summary>
        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void TestPlatformUsesCorrectTargetPlatformAssembly()
        {
            System.Reflection.Assembly unityAutoRegistrationAssembly;
#if NET40TESTS
            unityAutoRegistrationAssembly = typeof(Unity.AutoRegistration.AutoRegistration).Assembly;
#else
            unityAutoRegistrationAssembly = typeof(Unity.AutoRegistration.AutoRegistration).GetTypeInfo().Assembly;
#endif
            
            var targetFrameworkInformationAttribute = unityAutoRegistrationAssembly.GetCustomAttribute<TargetFrameworkInformationAttribute>();
            var runtimeTarget = unityAutoRegistrationAssembly.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>();

            if (targetFrameworkInformationAttribute == null)
                throw new Exception(nameof(TargetFrameworkInformationAttribute) + " not found.");

            var targetFramework = targetFrameworkInformationAttribute.TargetFramework;

            if (targetFramework == TargetFramework.Unknown)
                throw new Exception(nameof(TargetFrameworkInformationAttribute) + " is not implemented for all platforms.");

            TargetFramework expectedTargetFramework;
#if NET40TESTS
            expectedTargetFramework = TargetFramework.net40;
#elif NET45
            expectedTargetFramework = TargetFramework.net45;
#elif NETCOREAPP1_0
            expectedTargetFramework = TargetFramework.netstandard1_6;
#elif NETCOREAPP1_1
            expectedTargetFramework = TargetFramework.netstandard1_6;
#elif NETCOREAPP2_0
            expectedTargetFramework = TargetFramework.netstandard2_0;
#elif NETCOREAPP3_0
            expectedTargetFramework = TargetFramework.netstandard2_0;
#elif NET5_0
            expectedTargetFramework = TargetFramework.netstandard2_0;
#elif NET6_0
            expectedTargetFramework = TargetFramework.netstandard2_0;
#else
            throw new Exception("unknown testing platform");
#endif

            Assert.AreEqual(expectedTargetFramework, targetFramework);
        }

        private class RegisterEvent
        {
            public Type From { get; private set; }
            public Type To { get; private set; }
            public string Name { get; private set; }
            public ITypeLifetimeManager Lifetime { get; private set; }

            public RegisterEvent(Type from, Type to, string name, ITypeLifetimeManager lifetime)
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
                .LoadAssemblyFrom("MyFancyPlugin.dll")
                .ExcludeSystemAssemblies()
                .ExcludeAssemblies(a => a.GetName().FullName.Contains("Test"))
                .Include(If.ImplementsSingleInterface, Then.Register().AsSingleInterfaceOfType().UsingSingletonMode() )
                .Include(If.Implements<ILogger>, Then.Register().UsingPerCallMode())
                .Include(If.ImplementsITypeName, Then.Register().WithTypeName())
                .Include(If.Implements<ICustomerRepository>, Then.Register().WithName("Sample"))
                .Include(If.Implements<IOrderRepository>,
                         Then.Register().AsSingleInterfaceOfType().UsingPerCallMode())
                .Include(If.DecoratedWith<LoggerAttribute>,
                         Then.Register()
                             .As<IDisposable>()
                             .WithPartName(WellKnownAppParts.Logger)
                             .UsingLifetime<MyLifetimeManager>())
                .Exclude(t => t.Name.Contains("Trace"))
                .ApplyAutoRegistration();
        }
    }
}