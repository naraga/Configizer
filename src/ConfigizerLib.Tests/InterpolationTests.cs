using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConfigizerLib.Tests
{
    [TestClass]
    public class InterpolationTests
    {
        abstract class TestConfigBase : ConfigurationBase
        {
            public string ConnStr {
                get { return "user=${user};passwd=${passwd};srv=${server};catalog=${catalog}"; }
            }

            public string DateAndTime
            {
                get { return "Date=${dt:yyyy-MM-dd}, Time=${dt:HH:mm:ss}"; }
            }
        }

        class ConcreteTestConfig : TestConfigBase
        {
            // ReSharper disable MemberCanBePrivate.Local
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
            // ReSharper restore MemberCanBePrivate.Local
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
}
