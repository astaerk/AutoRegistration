using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Contract;
using Unity.AutoRegistration;

namespace Tests.AutoRegistration
{
    [TestClass]
    public class AutoRegistrationTests
    {
        [TestMethod]
        public void Example()
        {
            var container = new UnityContainer();

            container.RegisterType<ILogger, TestLogger>(new TransientLifetimeManager());
            container.RegisterType<ILogger, TestLogger>(new TransientLifetimeManager());

            container
                .ConfigureAutoRegistration()
                .Exclude(t => t.Name.Contains("Trace"))
                .Include(If.Implements<ILogger>,
                         Then.Register()
                             .UsingPerCallMode())
                .Include(If.DecoratedWith<LoggerAttribute>,
                         Then.Register()
                             .AsInterface<ILogger>()
                             .WithTypeName()
                             .UsingSingetonMode())
                .Include(If.Implements<ILogger>,
                         Then.Register().UsingLifetime<PerThreadLifetimeManager>())
                .Include(If.ImplementsITypeName,
                         Then.Register().UsingLifetime<PerThreadLifetimeManager>())
                .Include(If.Implements<ILogger>,
                         Then.Register().UsingPerCallMode())
                .Include(If.Implements<ICustomerRepository>,
                         Then.Register()
                             .AsSingleInterfaceOfType()
                             .UsingPerCallMode())
                .ApplyAutoRegistration();

            var logger = container.Resolve<ILogger>("TestLogger");
            var repository = container.Resolve<ICustomerRepository>();
        }
    }
}