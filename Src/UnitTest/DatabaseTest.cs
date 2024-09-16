using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrzAutoEntity.DataAccess;
using OrzAutoEntity.Helpers;

namespace UnitTest
{
    [TestClass]
    public class DatabaseTest : BaseTest
    {
        [TestMethod]
        public void TestOracleGetTableInfos()
        {
            TestGetTableInfos(GetDatabase(DatabaseType.Oracle));
        }

        [TestMethod]
        public void TestOracleGetColumnInfos()
        {
            TestGetColumnInfos(GetDatabase(DatabaseType.Oracle));
        }

        [TestMethod]
        public void TestOracleGetColumnInfosSpeed()
        {
            TestGetColumnInfosSpeed(GetDatabase(DatabaseType.Oracle));
        }

        [TestMethod]
        public void TestDmGetTableInfos()
        {
            TestGetTableInfos(GetDatabase(DatabaseType.Dm));
        }

        [TestMethod]
        public void TestDmGetColumnInfos()
        {
            TestGetColumnInfos(GetDatabase(DatabaseType.Dm));
        }

        [TestMethod]
        public void TestDmGetColumnInfosSpeed()
        {
            TestGetColumnInfosSpeed(GetDatabase(DatabaseType.Dm));
        }

        [TestMethod]
        public void TestGbaseGetTableInfos()
        {
            TestGetTableInfos(GetDatabase(DatabaseType.Gbase));
        }

        [TestMethod]
        public void TestGbaseGetColumnInfos()
        {
            TestGetColumnInfos(GetDatabase(DatabaseType.Gbase));
        }

        [TestMethod]
        public void TestGbaseGetColumnInfosSpeed()
        {
            TestGetColumnInfosSpeed(GetDatabase(DatabaseType.Gbase));
        }

        [TestMethod]
        public void TestSybaseGetTableInfos()
        {
            TestGetTableInfos(GetDatabase(DatabaseType.Sybase));
        }

        [TestMethod]
        public void TestSybaseGetColumnInfos()
        {
            TestGetColumnInfos(GetDatabase(DatabaseType.Sybase));
        }

        [TestMethod]
        public void TestSybaseGetColumnInfosSpeed()
        {
            TestGetColumnInfosSpeed(GetDatabase(DatabaseType.Sybase));
        }

        private void TestGetTableInfos(Database db)
        {
            var infos = db.GetTableInfos();
            WriteJson($"数量：{infos.Count}");
            WriteJson(infos);
        }

        private void TestGetColumnInfos(Database db, params string[] tableNames)
        {
            var tableInfos = db.GetTableInfos();
            if (tableNames.Length > 0)
            {
                tableInfos = tableInfos.Where(t => tableNames.Contains(t.Name, StringComparer.OrdinalIgnoreCase)).ToList();
            }

            tableInfos = db.FillColumnInfos(tableInfos);
            WriteJson(tableInfos);
        }

        private void TestGetColumnInfosSpeed(Database db)
        {
            var tableInfos = db.GetTableInfos();

            while (tableInfos.Count > 0)
            {
                Run($"数据量：{tableInfos.Count}", () =>
                {
                    db.FillColumnInfos(tableInfos);
                });
                tableInfos = tableInfos.Take(tableInfos.Count / 2).ToList();
            }

            Run($"数据量：0", () =>
            {
                db.FillColumnInfos(null);
            });
        }

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

        private Database GetDatabase(DatabaseType type)
        {
            var config = ConfigHelper.GetDatabaseConfig(type.ToString());
            return DatabaseFactory.GetDatabase(config.ConnString, type);
        }
    }
}
