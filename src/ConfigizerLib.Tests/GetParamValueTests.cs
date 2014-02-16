using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConfigizerLib.Tests
{
    [TestClass]
    public class GetParamValueTests
    {
        // ReSharper disable UnusedField.Compiler
        // ReSharper disable UnusedField.Compiler
        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedField.Compiler
        // ReSharper disable UnusedField.Compiler
        class A
        {
            public int F1 = 123;
            private int F2 = 123;
            protected int F3 = 123;
            protected int F4 = 12;
            protected int F5{get { return 12; }}
        }

        class B : A
        {
            protected int F4 = 123;
            protected int F5 = 123;
        }

        [TestMethod]
        public void GetPublicFieldFromBase_CaseInsensitiveTest()
        {
            var b = new B();
            Assert.AreEqual(123, b.GetParamValue("F1"));
            Assert.AreEqual(123, b.GetParamValue("f1"));
        }

        [TestMethod, ExpectedException(typeof(KeyNotFoundException))]
        public void FailOnGetingPrivateFieldFromBase()
        {
            var b = new B();
            Assert.AreEqual(123, b.GetParamValue("F2"));
        }

        [TestMethod]
        public void GetDefaultValueOnGetingPrivateFieldFromBase()
        {
            var b = new B();
            Assert.AreEqual(42, b.GetParamValue("F2", 42));
        }

        [TestMethod]
        public void GetValueFromChildClassIfConflictWithBase()
        {
            var b = new B();
            Assert.AreEqual(123, b.GetParamValue("F4"));
        }

        [TestMethod]
        public void PropertiesHaveHigherPriorityThanFieldsTest()
        {
            var b = new B();
            Assert.AreEqual(12, b.GetParamValue("F5"));
        }
    }
}
