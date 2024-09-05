using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrzAutoEntity.DataAccess;

namespace UnitTest
{
    [TestClass]
    public class DatabaseTest
    {
        [TestMethod]
        public void TestOracleGetTableInfos()
        {
            var db = DatabaseFactory.GetDatabase(Config.OracleConn, DatabaseType.Oracle);
            var infos = db.GetTableInfos();
            WriteJson(infos);
        }

        [TestMethod]
        public void TestDmGetTableInfos()
        {
            var db = DatabaseFactory.GetDatabase(Config.DmConn, DatabaseType.Dm);
            var infos = db.GetTableInfos();
            WriteJson(infos);
        }

        private void WriteJson(object obj)
        {
            Console.WriteLine(obj.ToJson());
        }
    }
}
