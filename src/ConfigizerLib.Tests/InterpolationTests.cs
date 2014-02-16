using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConfigizerLib.Tests
{
    [TestClass]
    public class InterpolationTests
    {
        abstract class TestConfigBase : ConfigurationBase
        {
            // ReSharper disable UnusedMember.Local
            public string ConnStr {
                get { return "user=${user};passwd=${passwd};srv=${server};catalog=${catalog}"; }
            }

            public string DateAndTime
            {
                get { return "Date=${dt:yyyy-MM-dd}, Time=${dt:HH:mm:ss}"; }
            }
            // ReSharper restore UnusedMember.Local
        }

        class ConcreteTestConfig : TestConfigBase
        {
            // ReSharper disable MemberCanBePrivate.Local
            // ReSharper disable NotAccessedField.Local
            // ReSharper disable InconsistentNaming
            // ReSharper disable InconsistentNaming
            public string User;
            private string Passwd;
            public string Server { get; set; }
            private string Catalog { get; set; }

            public ConcreteTestConfig(string user, string passwd, string server, string catalog)
            {
                User = user;
                Passwd = passwd;
                Server = server;
                Catalog = catalog;
            }

            private DateTime dt;
            public ConcreteTestConfig(DateTime dt)
            {
                this.dt = dt;
            }
            // ReSharper restore InconsistentNaming
            // ReSharper restore InconsistentNaming
            // ReSharper restore MemberCanBePrivate.Local
            // ReSharper restore NotAccessedField.Local
        }

        [TestMethod]
        public void TestStrings()
        {
            var c = new ConcreteTestConfig("john", "passs", "jumbosrv01", "northwind");
            Assert.AreEqual("user=john;passwd=passs;srv=jumbosrv01;catalog=northwind", c.GetParamValue("ConnStr"));
        }

        [TestMethod]
        public void TestWithFormatting()
        {
            var c = new ConcreteTestConfig(new DateTime(1981, 1, 28, 12, 34, 56));
            Assert.AreEqual("Date=1981-01-28, Time=12:34:56", c.GetParamValue("DateAndTime"));
        }
    }

    //[TestClass]
    //public class InterpolationWithHierarchyTests
    //{
    //    class A { protected string P1 = "A.P1";}
    //    class B : A { protected string P1 = "B.P1"; protected string P2 = "B.P2";}
    //    class C : B { protected string P1 = "C.P1";}

    //    [TestMethod]
    //    public void HierarchyTests()
    //    {
    //        var c = new C();
    //        Assert.AreEqual("C.P1", c.Interpolate(""));
    //    }
    //}
}
