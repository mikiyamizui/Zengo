using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using Serilog;
using System;
using System.Linq;
using Zengo;
using Zengo.CsvFile;
using Zengo.TextFile;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        private const string TableName = "zengo_test";

        [TestMethod]
        public void TestMethod1()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.File("sql.log")
                .CreateLogger();

            SqlTools.SqlLogAction = (sql, parameters) => logger.Information("{Sql} [{Parameters}]",
                sql, string.Join(", ", parameters.Select(p => $"{p.Name}:\"{p.Value}\"")));

            SqlTools.SelectLogAction = (sql, keys) => logger.Information("{Sql} {@Keys}", sql, keys);

            SqlTools.InsertLogAction = (sql, keys) => logger.Information("{Sql} {@Keys}", sql, keys);

            SqlTools.UpdateLogAction = (sql, updates, keys) => logger.Information("{Sql} {@Updates} {@Keys}", sql, updates, keys);

            SqlTools.DropLogAction = (sql, keys) => logger.Information("{Sql} {@Keys}", sql, keys);

            using (var conn = new NpgsqlConnection("Server=localhost;Port=5432;Database=test;UserId=postgres;Password=postgres;Enlist=true;"))
            {
                conn.Open();

                CreateTable(conn);
                Insert(conn, new { product_name = "Apple", product_price = 100, supplier_id = 1, created_by = "John", updated_by = "John" });
                Insert(conn, new { product_name = "Orange", product_price = 150, supplier_id = 2, created_by = "John", updated_by = "John" });
                Insert(conn, new { product_name = "Melon", product_price = 200, supplier_id = 3, created_by = "John", updated_by = "John" });

                var zl = new ConfigManager<NpgsqlDataAdapter>(conn);

                using (zl.Table(TableName, "order by product_id").AsTextFile().AsCsvFile())
                {
                    Update(conn, new { product_price = 165, updated_at = DateTime.Now, updated_by = "Sam" }, new { product_id = 1 });
                }
            }
        }

        private void CreateTable(NpgsqlConnection conn)
        {
            const string sql = @"
drop table if exists zengo_test;
create table public.zengo_test
(
    product_id integer not null generated always as identity (increment 1 start 1 minvalue 1),
    product_name character varying(30) not null,
    product_price numeric(10) not null,
    supplier_id integer,
    created_at timestamp with time zone default now(),
    created_by character varying(20) default null,
    updated_at timestamp with time zone default now(),
    updated_by character varying(20) default null,
    constraint products_pkey primary key (product_id)
);";
            SqlTools.GenerateSqlCommand(conn, sql).ExecuteNonQuery();
        }

        private void Insert<T>(NpgsqlConnection conn, T parameters)
            where T : class
        {
            SqlTools.GenerateInsertCommand(conn, TableName, parameters).ExecuteNonQuery();
        }

        private void Update<TParameterObject, TKeyObject>(NpgsqlConnection conn, TParameterObject parameters, TKeyObject keys)
        {
            SqlTools.GenerateUpdateCommand(conn, TableName, parameters, keys).ExecuteNonQuery();
        }
    }
}