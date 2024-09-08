using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrzAutoEntity.DataAccess;
using OrzAutoEntity.Helpers;

namespace UnitTest
{
    [TestClass]
    public class DatabaseTest
    {
        public DatabaseTest()
        {
            ConfigHelper.Init("");
        }

        #region TestGetTableInfos
        [TestMethod]
        public void TestOracleGetTableInfos()
        {
            TestGetTableInfos(GetOracleDatabase());
        }

        [TestMethod]
        public void TestDmGetTableInfos()
        {
            TestGetTableInfos(GetDmDatabase());
        }

        private void TestGetTableInfos(Database db)
        {
            var infos = db.GetTableInfos();
            WriteJson(infos);
        }
        #endregion

        #region TestGetColumnInfos
        [TestMethod]
        public void TestOracleGetColumnInfos()
        {
            TestGetColumnInfos(GetOracleDatabase());
        }

        [TestMethod]
        public void TestDmGetColumnInfos()
        {
            TestGetColumnInfos(GetDmDatabase());
        }

        private void TestGetColumnInfos(Database db)
        {
            var tableInfos = db.GetTableInfos();
            tableInfos = db.GetColumnInfos(tableInfos);
            WriteJson(tableInfos);
        }
        #endregion

        #region TestGetColumnInfosSpeed
        [TestMethod]
        public void TestOracleGetColumnInfosSpeed()
        {
            TestGetColumnInfosSpeed(GetOracleDatabase());
        }

        [TestMethod]
        public void TestDmGetColumnInfosSpeed()
        {
            TestGetColumnInfosSpeed(GetDmDatabase());
        }

        private void TestGetColumnInfosSpeed(Database db)
        {
            var tableInfos = db.GetTableInfos();

            while (tableInfos.Count > 0)
            {
                Run($"数据量：{tableInfos.Count}", () =>
                {
                    db.GetColumnInfos(tableInfos);
                });
                tableInfos = tableInfos.Take(tableInfos.Count / 2).ToList();
            }

            Run($"数据量：0", () =>
            {
                db.GetColumnInfos(null);
            });
        }
        #endregion

        private void WriteJson(object obj)
        {
            Console.WriteLine(obj.ToJson());
        }

        private void Run(string name, Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            Console.WriteLine($"{name}，耗时:{sw.ElapsedMilliseconds}ms");
        }

        private Database GetOracleDatabase()
        {
            return DatabaseFactory.GetDatabase(Config.OracleConn, DatabaseType.Oracle);
        }

        private Database GetDmDatabase()
        {
            return DatabaseFactory.GetDatabase(Config.DmConn, DatabaseType.Dm);
        }
    }
}
