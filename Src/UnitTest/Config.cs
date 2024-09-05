using System.IO;

namespace UnitTest
{
    static class Config
    {
        public static string OracleConn;
        public static string DmConn;

        static Config()
        {
            var lines = File.ReadAllLines("OrzAutoEntity.config");
            foreach (var line in lines)
            {
                if (line.StartsWith($"{nameof(OracleConn)}=")) OracleConn = line.Substring(line.IndexOf("=") + 1);
                if (line.StartsWith($"{nameof(DmConn)}=")) DmConn = line.Substring(line.IndexOf("=") + 1);
            }
        }
    }
}
