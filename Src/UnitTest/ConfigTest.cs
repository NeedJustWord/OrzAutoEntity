using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrzAutoEntity.Helpers;

namespace UnitTest
{
    [TestClass]
    public class ConfigTest : BaseTest
    {
        [TestMethod]
        public void TestInitConfig()
        {
            Console.WriteLine(ConfigHelper.Templates.ToJson());
            Console.WriteLine(ConfigHelper.Databases.ToJson());
            Console.WriteLine(ConfigHelper.Filters.ToJson());
            Console.WriteLine(TypeMapping.Mapping.ToJson());
        }
    }
}
