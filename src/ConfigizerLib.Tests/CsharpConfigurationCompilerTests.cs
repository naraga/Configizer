using System.Collections.Generic;
using ConfigizerLib.Compilation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConfigizerLib.Tests
{
    [TestClass]
    public class CsharpConfigurationCompilerTests
    {
        readonly CsharpConfigurationCompiler _compiler = new CsharpConfigurationCompiler();
        
        [TestMethod]
        public void NoHierarchyNoAdditionalProperties()
        {
            var cfg = _compiler.Compile(
                new ConfigurationFileInfo
                {
                    Base = null,
                    Name = "MyCfg",
                    Contents = @"string p1 = ""hello""; string p2=""${p1} world!"";"
                }, null);
            Assert.AreEqual("hello", cfg.GetParamValue("p1"));
            Assert.AreEqual("hello world!", cfg.GetParamValue("p2"));
        }

        [TestMethod]
        public void WithAdditionalPropertiesNoHierarchy()
        {
            var cfg = _compiler.Compile(
                new ConfigurationFileInfo
                {
                    Base = null,
                    Name = "MyCfg",
                    Contents =
                        @"
protected abstract string p1{get;}
public string p2=""${p1} world!"";"
                }, new Dictionary<string, string>{{"p1", "hello"}});
            Assert.AreEqual("hello world!", cfg.GetParamValue("p2"));
        }

        [TestMethod]
        public void HierarchyAndAdditionalProperties()
        {
            var baseCfgInfo = new ConfigurationFileInfo
            {
                Base = null,
                Name = "BaseCfg",
                Contents = @"
protected abstract string Name {get;}
protected string Greeting {get {return ""hello ${name}!"";}}
"
            };

            var myCfgInfo = new ConfigurationFileInfo
            {
                Base = baseCfgInfo,
                Name = "MyCfg",
                Contents = @"protected override string Name {get{return ""World"";}}"
            };
            var cfg = _compiler.Compile(
                myCfgInfo, new Dictionary<string, string>{{"Name", "Boris"}});
            Assert.AreEqual("hello Boris!", cfg.GetParamValue("Greeting"));
        }

    }
}
