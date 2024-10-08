﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AdoNetCore.AseClient;
using Dm;
using MySql.Data.MySqlClient;
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
                table.Columns.ForEach(ColumnTypeMapping);
            }
            return tableInfos;
        }

        protected void ColumnTypeMapping(ColumnInfo column)
        {
            if (TypeMapping.IsNumber(DatabaseType, column.DbType))
            {
                column.DbType = $"{column.DbType}({column.Precision},{column.Scale})";
            }
            column.ClrType = TypeMapping.GetClrType(DatabaseType, column.DbType);
        }

        protected TableInfo GetTableInfo(IDataReader reader)
        {
            return new TableInfo
            {
                Name = reader["table_name"].AsString(),
                IsView = reader["table_type"].AsBool(),
                Comment = reader["comments"].AsString().Trim(),
            };
        }

        protected ColumnInfo GetColumnInfo(IDataReader reader)
        {
            return new ColumnInfo
            {
                TableName = reader["table_name"].AsString(),
                Name = reader["column_name"].AsString(),
                Comment = reader["comments"].AsString().Trim(),
                DbType = reader["data_type"].AsString(),
                Length = reader["data_length"].AsInt(),
                Precision = reader["data_precision"].AsInt(),
                Scale = reader["data_scale"].AsInt(),
                AllowNull = reader["nullable"].AsBool(),
                IsKey = reader["is_key"].AsBool(),
                Identity = reader["identity_column"].AsBool(),
            };
        }

        protected IEnumerable<T> ExecuteReader<T>(string sql, object param, Func<IDataReader, T> action)
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = CreateCommand(conn, sql, param))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return action(reader);
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
            var sql = @"select t.table_name,case when t.table_type='VIEW' then 'Y' else 'N' end as table_type,t.comments from user_tab_comments t order by t.table_name";
            var result = ExecuteReader(sql, null, GetTableInfo).ToList();
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
            var columns = ExecuteReader(sql, param, GetColumnInfo).ToList();

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
            var sql = @"select t.table_name,case when t.table_type='VIEW' then 'Y' else 'N' end as table_type,t.comments from user_tab_comments t order by t.table_name";
            var result = ExecuteReader(sql, null, GetTableInfo).ToList();
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
            var columns = ExecuteReader(sql, param, GetColumnInfo).ToList();

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
            //系统表t.tabid<=99，用户表t.tabid>99
            var sql = @"SELECT t.tabname as table_name,case when t.tabtype='V' then 'Y' else 'N' end as table_type,c.comments FROM systables t LEFT JOIN syscomments c ON t.tabname=c.tabname AND t.tabtype=c.tabtype WHERE t.tabtype IN ('T','V') AND t.tabid>99 ORDER BY t.tabname";
            var result = ExecuteReader(sql, null, GetTableInfo).ToList();
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
            var columns = ExecuteReader(sql, param, GetColumnInfo).ToList();

            return Handle(tableInfos, columns);
        }
    }

    class SybaseDatabase : Database
    {
        protected override DatabaseType DatabaseType => DatabaseType.Sybase;

        public SybaseDatabase(string connStr) : base(connStr)
        {
        }

        protected override IDbConnection GetConnection()
        {
            return new AseConnection(connStr);
        }

        public override List<TableInfo> GetTableInfos()
        {
            var sql = @"SELECT t.name as table_name,case when t.type='V' then 'Y' else 'N' end as table_type,'' as comments FROM sysobjects t WHERE t.type IN ('U','V') ORDER BY t.name";
            var result = ExecuteReader(sql, null, GetTableInfo).ToList();
            return result;
        }

        public override List<TableInfo> FillColumnInfos(List<TableInfo> tableInfos)
        {
            Dictionary<string, object> param;
            string where;
            if (tableInfos?.Count > 0)
            {
                GetParamSql(tableInfos, "@", out var paramSql, out param);
                where = $"   AND o.name IN ({paramSql})";
            }
            else
            {
                param = null;
                where = "";
            }

            var sql = $@"
SELECT o.name AS table_name,
       c.name AS column_name,
       '' AS comments,
       t.name data_type,
       c.length AS data_length,
       c.prec data_precision,
       c.scale data_scale,
       CASE
         WHEN c.status &8 = 0 THEN
          'N'
         ELSE
          'Y'
       END AS nullable,
       CASE
         WHEN c.status &128 = 0 THEN
          'N'
         ELSE
          'Y'
       END AS identity_column,
       CASE
         WHEN index_col(o.name, 1, 1) = c.name OR
              index_col(o.name, 1, 2) = c.name OR
              index_col(o.name, 1, 3) = c.name OR
              index_col(o.name, 1, 4) = c.name OR
              index_col(o.name, 1, 5) = c.name OR
              index_col(o.name, 1, 6) = c.name OR
              index_col(o.name, 1, 7) = c.name OR
              index_col(o.name, 1, 8) = c.name OR
              index_col(o.name, 1, 9) = c.name OR
              index_col(o.name, 1, 10) = c.name OR
              index_col(o.name, 1, 11) = c.name OR
              index_col(o.name, 1, 12) = c.name OR
              index_col(o.name, 1, 13) = c.name OR
              index_col(o.name, 1, 14) = c.name OR
              index_col(o.name, 1, 15) = c.name OR
              index_col(o.name, 1, 16) = c.name OR
              index_col(o.name, 1, 17) = c.name OR
              index_col(o.name, 1, 18) = c.name OR
              index_col(o.name, 1, 19) = c.name OR
              index_col(o.name, 1, 20) = c.name OR
              index_col(o.name, 1, 21) = c.name OR
              index_col(o.name, 1, 22) = c.name OR
              index_col(o.name, 1, 23) = c.name OR
              index_col(o.name, 1, 24) = c.name OR
              index_col(o.name, 1, 25) = c.name OR
              index_col(o.name, 1, 26) = c.name OR
              index_col(o.name, 1, 27) = c.name OR
              index_col(o.name, 1, 28) = c.name OR
              index_col(o.name, 1, 29) = c.name OR
              index_col(o.name, 1, 30) = c.name OR
              index_col(o.name, 1, 31) = c.name OR
              index_col(o.name, 1, 32) = c.name OR
              index_col(o.name, 1, 33) = c.name OR
              index_col(o.name, 1, 34) = c.name OR
              index_col(o.name, 1, 35) = c.name OR
              index_col(o.name, 1, 36) = c.name OR
              index_col(o.name, 1, 37) = c.name OR
              index_col(o.name, 1, 38) = c.name OR
              index_col(o.name, 1, 39) = c.name OR
              index_col(o.name, 1, 40) = c.name OR
              index_col(o.name, 1, 41) = c.name OR
              index_col(o.name, 1, 42) = c.name OR
              index_col(o.name, 1, 43) = c.name OR
              index_col(o.name, 1, 44) = c.name OR
              index_col(o.name, 1, 45) = c.name OR
              index_col(o.name, 1, 46) = c.name OR
              index_col(o.name, 1, 47) = c.name OR
              index_col(o.name, 1, 48) = c.name OR
              index_col(o.name, 1, 49) = c.name OR
              index_col(o.name, 1, 50) = c.name OR
              index_col(o.name, 1, 51) = c.name OR
              index_col(o.name, 1, 52) = c.name OR
              index_col(o.name, 1, 53) = c.name OR
              index_col(o.name, 1, 54) = c.name OR
              index_col(o.name, 1, 55) = c.name OR
              index_col(o.name, 1, 56) = c.name OR
              index_col(o.name, 1, 57) = c.name OR
              index_col(o.name, 1, 58) = c.name OR
              index_col(o.name, 1, 59) = c.name OR
              index_col(o.name, 1, 60) = c.name OR
              index_col(o.name, 1, 61) = c.name OR
              index_col(o.name, 1, 62) = c.name OR
              index_col(o.name, 1, 63) = c.name OR
              index_col(o.name, 1, 64) = c.name THEN
          'Y'
         ELSE
          'N'
       END AS is_key
  FROM syscolumns c
  JOIN sysobjects o
    ON c.id = o.id
  JOIN systypes t
    ON c.usertype = t.usertype
 WHERE o.type IN ('U', 'V')
{where}
 ORDER BY o.name, c.colid";
            var columns = ExecuteReader(sql, param, GetColumnInfo).ToList();

            return Handle(tableInfos, columns);
        }
    }

    class MySqlDatabase : Database
    {
        protected override DatabaseType DatabaseType => DatabaseType.MySql;

        private string tableSchema;

        public MySqlDatabase(string connStr) : base(connStr)
        {
            tableSchema = connStr.SplitRemoveEmptyEntries(';')
                .Select(t => t.Trim())
                .FirstOrDefault(t => t.StartsWith("Database", StringComparison.OrdinalIgnoreCase));
            tableSchema = tableSchema.Substring(tableSchema.IndexOf('=') + 1).TrimStart();
        }

        protected override IDbConnection GetConnection()
        {
            return new MySqlConnection(connStr);
        }

        public override List<TableInfo> GetTableInfos()
        {
            var param = new Dictionary<string, object>
            {
                { "@Schema",tableSchema}
            };
            var sql = @"select t.TABLE_NAME,case when t.TABLE_TYPE='VIEW' then 'Y' else 'N' end as TABLE_TYPE,t.TABLE_COMMENT as comments from information_schema.tables t where t.TABLE_SCHEMA=@Schema and t.TABLE_TYPE in ('BASE TABLE','VIEW') order by t.TABLE_NAME";
            var result = ExecuteReader(sql, param, GetTableInfo).ToList();
            return result;
        }

        public override List<TableInfo> FillColumnInfos(List<TableInfo> tableInfos)
        {
            Dictionary<string, object> param;
            string where;
            if (tableInfos?.Count > 0)
            {
                GetParamSql(tableInfos, "@", out var paramSql, out param);
                where = $"   and c.TABLE_NAME in ({paramSql})";
            }
            else
            {
                param = new Dictionary<string, object>();
                where = "";
            }
            param["@Schema"] = tableSchema;

            var sql = $@"
select c.TABLE_NAME,
       c.COLUMN_NAME,
       c.COLUMN_COMMENT as COMMENTS,
       case
         when instr(c.COLUMN_TYPE, 'unsigned') = 0 then
          c.DATA_TYPE
         else
          concat(c.DATA_TYPE, ' unsigned')
       end as DATA_TYPE,
       c.CHARACTER_MAXIMUM_LENGTH as DATA_LENGTH,
       c.NUMERIC_PRECISION as DATA_PRECISION,
       c.NUMERIC_SCALE as DATA_SCALE,
       c.IS_NULLABLE as NULLABLE,
       case
         when instr(c.EXTRA, 'auto_increment') = 0 then
          'N'
         else
          'Y'
       end as IDENTITY_COLUMN,
       case
         when instr(c.COLUMN_KEY, 'PRI') = 0 then
          'N'
         else
          'Y'
       end as IS_KEY
  from information_schema.columns c
 where c.TABLE_SCHEMA = @Schema
{where}
 order by c.TABLE_NAME, c.ORDINAL_POSITION";
            var columns = ExecuteReader(sql, param, GetColumnInfo).ToList();

            return Handle(tableInfos, columns);
        }
    }

    class SqliteDatabase : Database
    {
        protected override DatabaseType DatabaseType => DatabaseType.Sqlite;

        private Regex lengthRegex = new Regex(@"\(\s*(?<length>\d+)\s*,?\s*(?<scale>\d+)?\s*\)");

        public SqliteDatabase(string connStr) : base(connStr)
        {
        }

        protected override IDbConnection GetConnection()
        {
            return new SQLiteConnection(connStr);
        }

        public override List<TableInfo> GetTableInfos()
        {
            var sql = @"SELECT t.name AS table_name,CASE WHEN t.type='view' THEN 'Y' ELSE 'N' END AS table_type,t.sql FROM sqlite_master t WHERE t.type IN ('table','view') AND t.name<>'sqlite_sequence' ORDER BY t.name";
            var result = ExecuteReader(sql, null, reader =>
            {
                return new TableInfo
                {
                    Name = reader["table_name"].AsString(),
                    IsView = reader["table_type"].AsBool(),
                    Comment = "",
                    Sql = reader["sql"].AsString().ToUpper(),
                };
            }).ToList();
            return result;
        }

        public override List<TableInfo> FillColumnInfos(List<TableInfo> tableInfos)
        {
            if (tableInfos == null || tableInfos.Count == 0)
            {
                tableInfos = GetTableInfos();
            }

            if (tableInfos.Count == 0)
            {
                return tableInfos;
            }

            foreach (var table in tableInfos)
            {
                var sql = $"pragma table_info('{table.Name}')";
                table.Columns = ExecuteReader(sql, null, reader =>
                {
                    var column = new ColumnInfo
                    {
                        TableName = table.Name,
                        Name = reader["name"].AsString(),
                        Comment = "",
                        DbType = reader["type"].AsString(),
                        AllowNull = reader["notnull"].AsInt() == 0,
                        IsKey = reader["pk"].AsInt() == 1,
                    };

                    if (table.IsView == false) DecodeIdentity(column, table.Sql);
                    DecodeDbType(column);
                    ColumnTypeMapping(column);

                    return column;
                }).ToList();
            }

            return tableInfos;
        }

        private void DecodeIdentity(ColumnInfo column, string sql)
        {
            var regex = new Regex($@"\b{column.Name.ToUpper()}\b[^,]+AUTOINCREMENT");
            column.Identity = regex.IsMatch(sql);
        }

        private void DecodeDbType(ColumnInfo column)
        {
            var match = lengthRegex.Match(column.DbType);
            if (match.Success)
            {
                column.DbType = column.DbType.Substring(0, column.DbType.IndexOf('(')).Trim();
                column.Length = int.Parse(match.Groups["length"].Value);
                column.Precision = column.Length;
                var scale = match.Groups["scale"].Value;
                if (scale.IsNotNullAndEmpty())
                {
                    column.Scale = int.Parse(scale);
                }
            }
        }
    }
}
