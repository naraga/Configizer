using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Configizer.Tests
{
    [TestClass]
    public class ConfigizerManualRunner
    {
        [TestMethod]
        public void Run()
        {
            Program.Apply(@"config\PROD01.csconfig", new [] {"DbPassword=secret password"});
        }
    }
}