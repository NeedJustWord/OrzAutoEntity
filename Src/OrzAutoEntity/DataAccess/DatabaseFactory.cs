using System;

namespace OrzAutoEntity.DataAccess
{
    public static class DatabaseFactory
    {
        public static Database GetDatabase(string connStr, DatabaseType type)
        {
            switch (type)
            {
                case DatabaseType.Oracle:
                    return new OracleDatabase(connStr);
                case DatabaseType.Dm:
                    return new DmDatabase(connStr);
                case DatabaseType.Gbase:
                    return new GbaseDatabase(connStr);
                default:
                    throw new Exception("不支持的数据库类型");
            }
        }
    }
}
