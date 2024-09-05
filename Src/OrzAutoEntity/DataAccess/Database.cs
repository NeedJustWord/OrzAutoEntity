using System;
using System.Collections.Generic;
using System.Data;
using Dm;
using Oracle.ManagedDataAccess.Client;
using OrzAutoEntity.Modes;

namespace OrzAutoEntity.DataAccess
{
    public abstract class Database
    {
        protected readonly string connStr;

        public Database(string connStr)
        {
            this.connStr = connStr;
        }

        protected abstract IDbConnection GetConnection();
        public abstract List<TableInfo> GetTableInfos();
        public abstract void GetColumnInfos(ref List<TableInfo> tableInfos);

        protected void ExecuteReader(string sql, object param, Action<IDataReader> action)
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = CreateCommand(conn, sql, param))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            action(reader);
                        }
                    }
                }
            }
        }

        private IDbConnection OpenConnection()
        {
            var conn = GetConnection();
            conn.Open();
            return conn;
        }

        private IDbCommand CreateCommand(IDbConnection conn, string sql, object param)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.SetParameters(param);
            return cmd;
        }
    }

    class OracleDatabase : Database
    {
        public OracleDatabase(string connStr) : base(connStr)
        {
        }

        protected override IDbConnection GetConnection()
        {
            return new OracleConnection(connStr);
        }

        public override List<TableInfo> GetTableInfos()
        {
            var result = new List<TableInfo>();
            var sql = @"select t.table_name,t.table_type,t.comments from user_tab_comments t";
            ExecuteReader(sql, null, reader =>
            {
                result.Add(new TableInfo
                {
                    Name = reader["TABLE_NAME"].AsString(),
                    IsView = reader["TABLE_TYPE"].AsString() == "VIEW",
                    Comment = reader["COMMENTS"].AsString(),
                });
            });
            return result;
        }

        public override void GetColumnInfos(ref List<TableInfo> tableInfos)
        {
            throw new NotImplementedException();
        }
    }

    class DmDatabase : Database
    {
        public DmDatabase(string connStr) : base(connStr)
        {
        }

        protected override IDbConnection GetConnection()
        {
            return new DmConnection(connStr);
        }

        public override List<TableInfo> GetTableInfos()
        {
            var result = new List<TableInfo>();
            var sql = @"select t.table_name,t.table_type,t.comments from user_tab_comments t";
            ExecuteReader(sql, null, reader =>
            {
                result.Add(new TableInfo
                {
                    Name = reader["TABLE_NAME"].AsString(),
                    IsView = reader["TABLE_TYPE"].AsString() == "VIEW",
                    Comment = reader["COMMENTS"].AsString(),
                });
            });
            return result;
        }

        public override void GetColumnInfos(ref List<TableInfo> tableInfos)
        {
            throw new NotImplementedException();
        }
    }
}
