using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrzAutoEntity.DataAccess;
using OrzAutoEntity.Helpers;
using OrzAutoEntity.Services;

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
        public void TestDmGenerate()
        {
            TestGenerate(DatabaseType.Dm, "PRM_VERSION");
        }

        [TestMethod]
        public void TestOracleGenerateAll()
        {
            TestGenerate(DatabaseType.Oracle);
        }

        [TestMethod]
        public void TestDmGenerateAll()
        {
            TestGenerate(DatabaseType.Dm);
        }

        private void TestGenerate(DatabaseType type, params string[] tableNames)
        {
            var dbName = type.ToString();
            var dbConfig = ConfigHelper.GetDatabaseConfig(dbName);
            var templateConfig = ConfigHelper.GetTemplateConfig(dbConfig.TemplateId);
            var filterConfig = ConfigHelper.GetFilterConfig(dbConfig.FilterId);

            var db = DatabaseFactory.GetDatabase(dbConfig.ConnString, type);
            var tables = db.GetTableInfos().Filter(filterConfig);
            if (tableNames.Length > 0)
            {
                tables = tables.Where(t => tableNames.Contains(t.Name, StringComparer.OrdinalIgnoreCase)).ToList();
            }

            tables = db.FillColumnInfos(tables);

            var path = "";
            var dir = Path.Combine(path, dbConfig.Directory);
            DirectoryHelper.CreateDirectory(dir);
            foreach (var file in Directory.GetFiles(dir))
            {
                File.Delete(file);
            }

            foreach (var table in tables)
            {
                var file = Path.Combine(dir, $"{table.Name}.cs");
                var content = GenerateService.GetEntityContent(templateConfig.Content, table);
                GenerateService.SaveFile(file, content);
            }
        }
        #endregion
    }
}
