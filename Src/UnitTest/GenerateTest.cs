using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrzAutoEntity.DataAccess;
using OrzAutoEntity.Helpers;
using OrzAutoEntity.Modes;

namespace UnitTest
{
    [TestClass]
    public class GenerateTest : BaseTest
    {
        #region TestGenerate
        [TestMethod]
        public void TestOracleGenerate()
        {
            TestGenerate(DatabaseType.Oracle, "PRM_VERSION");
        }

        [TestMethod]
        public void TestOracleGenerateAll()
        {
            TestGenerate(DatabaseType.Oracle);
        }

        [TestMethod]
        public void TestDmGenerate()
        {
            TestGenerate(DatabaseType.Dm, "PRM_VERSION");
        }

        [TestMethod]
        public void TestDmGenerateAll()
        {
            TestGenerate(DatabaseType.Dm);
        }

        [TestMethod]
        public void TestGbaseGenerate()
        {
            TestGenerate(DatabaseType.Gbase, "PRMVERSION");
        }

        [TestMethod]
        public void TestGbaseGenerateAll()
        {
            TestGenerate(DatabaseType.Gbase);
        }

        [TestMethod]
        public void TestSybaseGenerate()
        {
            TestGenerate(DatabaseType.Sybase, "PRMVERSION");
        }

        [TestMethod]
        public void TestSybaseGenerateAll()
        {
            TestGenerate(DatabaseType.Sybase);
        }

        [TestMethod]
        public void TestMySqlGenerate()
        {
            TestGenerate(DatabaseType.MySql, "PRMVERSION");
        }

        [TestMethod]
        public void TestMySqlGenerateAll()
        {
            TestGenerate(DatabaseType.MySql);
        }

        [TestMethod]
        public void TestSqliteGenerate()
        {
            TestGenerate(DatabaseType.Sqlite, "PRMVERSION");
        }

        [TestMethod]
        public void TestSqliteGenerateAll()
        {
            TestGenerate(DatabaseType.Sqlite);
        }

        [TestMethod]
        public void TestSqlServerGenerate()
        {
            TestGenerate(DatabaseType.SqlServer, "tbSysUser");
        }

        [TestMethod]
        public void TestSqlServerGenerateAll()
        {
            TestGenerate(DatabaseType.SqlServer);
        }

        private void TestGenerate(DatabaseType type, params string[] tableNames)
        {
            var dbType = type.ToString();
            var dbConfig = ConfigHelper.GetDatabaseConfig(dbType);
            var templateConfigs = ConfigHelper.GetTemplateConfig(dbConfig.TemplateIds);
            var tableFilterConfigs = ConfigHelper.GetFilterConfig(dbConfig.FilterIds, FilterType.Table);

            var db = DatabaseFactory.GetDatabase(dbConfig.ConnString, type);
            var tables = db.GetTableInfos();
            FilterConfig.Handle(tableFilterConfigs, tables);
            if (tableNames.Length > 0)
            {
                tables = tables.Where(t => tableNames.Contains(t.Name, StringComparer.OrdinalIgnoreCase)).ToList();
            }

            tables = db.FillColumnInfos(tables);

            var tableFiles = TableFileInfo.GetTableFileInfos(templateConfigs, tables);
            var treeNode = TreeNode.CreateTreeNode(dbConfig, tableFiles);
            var projectFullPath = "";
            treeNode.GenerateFile(projectFullPath);
            Console.WriteLine($"数量：{tables.Count}");
            Console.WriteLine(tables.ToJson());
        }
        #endregion
    }
}
