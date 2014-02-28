using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ConfigizerLib.Tests
{

    [TestClass]
    public class ConfigurationLoaderTests
    {
        private const string ContentsLinux = "#base .\\base\npublic override string DbDataSource {get {return \"proddbsrv\\mssql2012\";}}\npublic override string DbPassword {get {return ThrowNotOverriden();}}";
        private const string ContentsWin = "#base .\\base\r\npublic override string DbDataSource {get {return \"proddbsrv\\mssql2012\";}}\r\npublic override string DbPassword {get {return ThrowNotOverriden();}}";

        [TestMethod]
        public void GetTagValuesLinux()
        {
            var values = ConfigurationLoader.GetTagValues(ContentsLinux, "base");
            Assert.IsTrue(values.Count() == 1);
            Assert.AreEqual(values[0], @".\base");
        }

        [TestMethod]
        public void GetTagValuesWin()
        {
            var values = ConfigurationLoader.GetTagValues(ContentsWin, "base");
            Assert.IsTrue(values.Count() == 1);
            Assert.AreEqual(values[0], @".\base");
        }

        [TestMethod]
        public void RemoveTagsLinux()
        {
            var contents = ConfigurationLoader.RemoveTags(ContentsLinux, "base");
            Assert.IsFalse(contents.Contains("base"));
        }

        [TestMethod]
        public void RemoveTagsWin()
        {
            var contents = ConfigurationLoader.RemoveTags(ContentsWin, "base");
            Assert.IsFalse(contents.Contains("base"));
        }
    }
}
