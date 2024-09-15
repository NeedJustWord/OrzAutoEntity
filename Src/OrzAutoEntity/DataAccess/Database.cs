using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using Dm;
using Oracle.ManagedDataAccess.Client;
using OrzAutoEntity.Helpers;
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

        protected abstract DatabaseType DatabaseType { get; }
        protected abstract IDbConnection GetConnection();
        public abstract List<TableInfo> GetTableInfos();
        public abstract List<TableInfo> FillColumnInfos(List<TableInfo> tableInfos);

        protected void GetParamSql(List<TableInfo> tableInfos, string prefix, out string paramSql, out Dictionary<string, object> param, bool hasParamName = true)
        {
            var builder = new StringBuilder();
            param = new Dictionary<string, object>(tableInfos.Count);

            int index = 0;
            string paramName;
            if (hasParamName)
            {
                foreach (var table in tableInfos)
                {
                    paramName = $"{prefix}P{index++}";
                    builder.Append($",{paramName}");
                    param[paramName] = table.Name;
                }
            }
            else
            {
                foreach (var table in tableInfos)
                {
                    paramName = $"{prefix}P{index++}";
                    builder.Append($",{prefix}");
                    param[paramName] = table.Name;
                }
            }

            paramSql = builder.ToString().Substring(1);
        }

        protected List<TableInfo> Handle(List<TableInfo> tableInfos, List<ColumnInfo> columnInfos)
        {
            if (tableInfos == null || tableInfos.Count == 0)
            {
                return columnInfos.GroupBy(t => t.TableName).Select(t => new TableInfo
                {
                    Name = t.Key,
                    Columns = t.ToList(),
                    Comment = "",
                }).ToList();
            }

            var dict = columnInfos.GroupBy(t => t.TableName).ToDictionary(t => t.Key, t => t.ToList());
            foreach (var table in tableInfos)
            {
                table.Columns = dict[table.Name];
                table.Columns.ForEach(t =>
                {
                    if (TypeMapping.IsNumber(DatabaseType, t.DbType))
                    {
                        t.DbType = $"{t.DbType}({t.Precision},{t.Scale})";
                    }
                    t.ClrType = TypeMapping.GetClrType(DatabaseType, t.DbType);
                });
            }
            return tableInfos;
        }

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
        protected override DatabaseType DatabaseType => DatabaseType.Oracle;

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
            var sql = @"select t.table_name,t.table_type,t.comments from user_tab_comments t order by t.table_name";
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

        public override List<TableInfo> FillColumnInfos(List<TableInfo> tableInfos)
        {
            Dictionary<string, object> param;
            string where1, where2;
            if (tableInfos?.Count > 0)
            {
                GetParamSql(tableInfos, ":", out var paramSql, out param);
                where1 = $" where t.table_name in ({paramSql})";
                where2 = $" and con.table_name in ({paramSql})";
            }
            else
            {
                param = null;
                where1 = "";
                where2 = "";
            }

            var columns = new List<ColumnInfo>();
            var sql = $@"
select t.table_name,
       t.column_name,
       c.comments,
       t.data_type,
       t.data_length,
       t.data_precision,
       t.data_scale,
       t.nullable,
       t.identity_column,
       case
         when k.column_name is null then
          'N'
         else
          'Y'
       end as is_key
  from user_tab_columns t
  join user_col_comments c
    on t.table_name = c.table_name
   and t.column_name = c.column_name
  left join (select con.table_name, col.column_name
               from user_constraints con, user_cons_columns col
              where con.constraint_name = col.constraint_name
                and con.constraint_type = 'P' {where2}) k
    on t.table_name = k.table_name
   and t.column_name = k.column_name
{where1}
 order by t.table_name, t.column_id";
            ExecuteReader(sql, param, reader =>
            {
                columns.Add(new ColumnInfo
                {
                    TableName = reader["table_name"].AsString(),
                    Name = reader["column_name"].AsString(),
                    Comment = reader["comments"].AsString(),
                    DbType = reader["data_type"].AsString(),
                    Length = reader["data_length"].AsInt(),
                    Precision = reader["data_precision"].AsInt(),
                    Scale = reader["data_scale"].AsInt(),
                    AllowNull = reader["nullable"].AsBool(),
                    IsKey = reader["is_key"].AsBool(),
                    Identity = reader["identity_column"].AsBool(),
                });
            });

            return Handle(tableInfos, columns);
        }
    }

    class DmDatabase : Database
    {
        protected override DatabaseType DatabaseType => DatabaseType.Dm;

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
            var sql = @"select t.table_name,t.table_type,t.comments from user_tab_comments t order by t.table_name";
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

        public override List<TableInfo> FillColumnInfos(List<TableInfo> tableInfos)
        {
            Dictionary<string, object> param;
            string where1, where2, where3;
            if (tableInfos?.Count > 0)
            {
                GetParamSql(tableInfos, ":", out var paramSql, out param);
                where1 = $" where t.table_name in ({paramSql})";
                where2 = $" and con.table_name in ({paramSql})";
                where3 = $" and b.table_name in ({paramSql})";
            }
            else
            {
                param = null;
                where1 = "";
                where2 = "";
                where3 = "";
            }

            var columns = new List<ColumnInfo>();
            var sql = $@"
select t.table_name,
       t.column_name,
       c.comments,
       t.data_type,
       t.data_length,
       t.data_precision,
       t.data_scale,
       t.nullable,
       case
         when i.column_name is null then
          'N'
         else
          'Y'
       end as identity_column,
       case
         when k.column_name is null then
          'N'
         else
          'Y'
       end as is_key
  from user_tab_columns t
  join user_col_comments c
    on t.table_name = c.table_name
   and t.column_name = c.column_name
  left join (select con.table_name, col.column_name
               from user_constraints con, user_cons_columns col
              where con.constraint_name = col.constraint_name
                and con.constraint_type = 'P' {where2}) k
    on t.table_name = k.table_name
   and t.column_name = k.column_name
  left join (select b.table_name, a.name as column_name
               from sys.syscolumns a, all_tables b, sys.sysobjects c
              where a.info2 &0x01 = 0x01
                and a.id = c.id
                and c.name = b.table_name {where3}) i
    on t.table_name = i.table_name
   and t.column_name = i.column_name
{where1}
 order by t.table_name, t.column_id";
            ExecuteReader(sql, param, reader =>
            {
                columns.Add(new ColumnInfo
                {
                    TableName = reader["table_name"].AsString(),
                    Name = reader["column_name"].AsString(),
                    Comment = reader["comments"].AsString(),
                    DbType = reader["data_type"].AsString(),
                    Length = reader["data_length"].AsInt(),
                    Precision = reader["data_precision"].AsInt(),
                    Scale = reader["data_scale"].AsInt(),
                    AllowNull = reader["nullable"].AsBool(),
                    IsKey = reader["is_key"].AsBool(),
                    Identity = reader["identity_column"].AsBool(),
                });
            });

            return Handle(tableInfos, columns);
        }
    }

    class GbaseDatabase : Database
    {
        protected override DatabaseType DatabaseType => DatabaseType.Gbase;

        public GbaseDatabase(string connStr) : base(connStr)
        {
        }

        protected override IDbConnection GetConnection()
        {
            return new OdbcConnection(connStr);
        }

        public override List<TableInfo> GetTableInfos()
        {
            var result = new List<TableInfo>();
            var sql = @"SELECT t.tabname,t.tabtype,c.comments FROM systables t LEFT JOIN syscomments c ON t.tabname=c.tabname AND t.tabtype=c.tabtype WHERE t.tabtype IN ('T','V') AND t.tabid>99 ORDER BY t.tabname";
            ExecuteReader(sql, null, reader =>
            {
                result.Add(new TableInfo
                {
                    Name = reader["tabname"].AsString(),
                    IsView = reader["tabtype"].AsString() == "V",
                    Comment = reader["comments"].AsString(),
                });
            });
            return result;
        }

        public override List<TableInfo> FillColumnInfos(List<TableInfo> tableInfos)
        {
            Dictionary<string, object> param;
            string where;
            if (tableInfos?.Count > 0)
            {
                GetParamSql(tableInfos, "?", out var paramSql, out param, false);
                where = $"   AND t.tabname IN ({paramSql})";
            }
            else
            {
                param = null;
                where = "";
            }

            var columns = new List<ColumnInfo>();
            var sql = $@"
SELECT t.tabname AS table_name,
       c.colname AS column_name,
       cc.comments,
       CASE
         WHEN instr(c.coltypename, '(') = 0 THEN
          c.coltypename
         ELSE
          substr(c.coltypename, 1, instr(c.coltypename, '(') - 1)
       END AS data_type,
       c.collength AS data_length,
       CASE
         WHEN c.coltypename IN ('DATE',
                                'SMALLINT',
                                'INTEGER',
                                'BIGINT',
                                'INT8',
                                'SERIAL',
                                'BIGSERIAL',
                                'SERIAL8',
                                'SMALLFLOAT',
                                'FLOAT') THEN
          c.collength
         WHEN c.coltypename IN ('DATETIME', 'INTERVAL') THEN
          bitand(c.collength, 240) / 16
         WHEN c.coltypename IN ('DECIMAL', 'MONEY') THEN
          floor(c.collength / 256)
         ELSE
          0
       END AS data_precision,
       CASE
         WHEN c.coltypename IN ('DATETIME', 'INTERVAL') THEN
          bitand(c.collength, 15)
         WHEN c.coltypename IN ('DECIMAL', 'MONEY') THEN
          mod(c.collength, 256)
         ELSE
          0
       END AS data_scale,
       CASE
         WHEN bitand(c.coltype, 256) = 256 THEN
          'N'
         ELSE
          'Y'
       END AS nullable,
       CASE
         WHEN c.coltypename IN ('SERIAL', 'BIGSERIAL', 'SERIAL8') THEN
          'Y'
         ELSE
          'N'
       END AS identity_column,
       CASE
         WHEN instr(con.part, ',' || to_char(c.colno) || ',') = 0 THEN
          'N'
         ELSE
          'Y'
       END AS is_key
  FROM syscolumnsext c
  JOIN systables t
    ON c.tabid = t.tabid
  LEFT JOIN syscolcomms cc
    ON c.tabid = cc.tabid
   AND c.colno = cc.colno
  LEFT JOIN (SELECT con.tabid,
                    ',' || ind.part1 || ',' || ind.part2 || ',' || ind.part3 || ',' ||
                    ind.part4 || ',' || ind.part5 || ',' || ind.part6 || ',' ||
                    ind.part7 || ',' || ind.part8 || ',' || ind.part9 || ',' ||
                    ind.part10 || ',' || ind.part11 || ',' || ind.part12 || ',' ||
                    ind.part13 || ',' || ind.part14 || ',' || ind.part15 || ',' ||
                    ind.part16 || ',' AS part
               FROM sysconstraints con, sysindexes ind
              WHERE con.idxname = ind.idxname
                AND con.constrtype = 'P'
                AND con.tabid > 99) con
    ON t.tabid = con.tabid
 WHERE t.tabtype IN ('T', 'V')
   AND t.tabid > 99
{where}
 ORDER BY t.tabname, c.colno";
            ExecuteReader(sql, param, reader =>
            {
                columns.Add(new ColumnInfo
                {
                    TableName = reader["table_name"].AsString(),
                    Name = reader["column_name"].AsString(),
                    Comment = reader["comments"].AsString(),
                    DbType = reader["data_type"].AsString(),
                    Length = reader["data_length"].AsInt(),
                    Precision = reader["data_precision"].AsInt(),
                    Scale = reader["data_scale"].AsInt(),
                    AllowNull = reader["nullable"].AsBool(),
                    IsKey = reader["is_key"].AsBool(),
                    Identity = reader["identity_column"].AsBool(),
                });
            });

            return Handle(tableInfos, columns);
        }
    }
}
