using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Lifetime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Contracts;
using Unity.AutoRegistration;
using Moq;
using Unity.Registration;

namespace Tests.AutoRegistration
{
    [TestClass]
    public class ExtensionFixture
    {
#if NET40TESTS
        private const string TESTCATEGORY = "NET40";
#else
        private const string TESTCATEGORY = "NETSTANDARD AND NET45";
#endif

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void GetAttributeWithInvalidAttribute_ShouldThrowException()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                typeof(ExtensionFixture).GetAttribute<TestMethodAttribute>();
            });
        }

        [TestMethod]
        [TestCategory(TESTCATEGORY)]
        public void GetAttributeWithValidAttribute_ShouldReturnAttribute()
        {
            var result = typeof(ExtensionFixture).GetAttribute<TestClassAttribute>();
            Assert.IsNotNull(result);
        }
        
    }
}
