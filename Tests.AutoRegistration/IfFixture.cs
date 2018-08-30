using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.AutoRegistration;

namespace Tests.AutoRegistration
{
    [TestClass]
    public class IfFixture
    {
#if NET40TESTS
        private const string TESTCATEGORY = "NET40";
#else
        private const string TESTCATEGORY = "NETSTANDARD AND NET45";
#endif

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void IfNotDecoratedWithAttribute_ShouldReturnFalse()
        {
            var result = typeof(TestLogger).DecoratedWith<IgnoreAttribute>();
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void IfDecoratedWithAttribute_ShouldReturnTrue()
        {
            var result = typeof(TestLogger).DecoratedWith<Contracts.LoggerAttribute>();
            Assert.IsTrue(result);
        }

    }
}
