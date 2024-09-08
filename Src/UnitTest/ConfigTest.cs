using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrzAutoEntity.Helpers;

namespace UnitTest
{
    [TestClass]
    public class ConfigTest
    {
        [TestMethod]
        public void TestInitConfig()
        {
            ConfigHelper.Init("");
            Console.WriteLine(ConfigHelper.Templates.ToJson());
            Console.WriteLine(ConfigHelper.Databases.ToJson());
            Console.WriteLine(TypeMapping.Mapping.ToJson());
        }
    }
}
