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
        [TestMethod]
        public void TestMethod1()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.File("sql.log")
                .CreateLogger();

            SqlTools.SqlLog = (sql, parameters) => logger.Information("{Sql} [{Parameters}]", sql, string.Join(", ", parameters.Select(p => $"{p.Name}:\"{p.Value}\"")));
            SqlTools.SelectLog = (sql, keys) => logger.Information("{Sql} {@Keys}", sql, keys);
            SqlTools.InsertLog = (sql, keys) => logger.Information("{Sql} {@Keys}", sql, keys);
            SqlTools.UpdateLog = (sql, updates, keys) => logger.Information("{Sql} {@Updates} {@Keys}", sql, updates, keys);
            SqlTools.DropLog = (sql, keys) => logger.Information("{Sql} {@Keys}", sql, keys);

            using (var connection = new NpgsqlConnection("Server=localhost;Port=5432;Database=test;UserId=postgres;Password=postgres;Enlist=true;"))
            {
                connection.Open();

                connection.ExecuteNonQuery(null, ddl);

                connection.ExecuteInsert(null, table,
                    new
                    {
                        product_name = "Apple",
                        product_price = 100,
                        supplier_id = 1,
                        created_by = "John",
                        updated_by = "John"
                    });

                connection.ExecuteInsert(null, table,
                    new
                    {
                        product_name = "Orange",
                        product_price = 150,
                        supplier_id = 2,
                        created_by = "John",
                        updated_by = "John"
                    });

                connection.ExecuteInsert(null, table,
                    new
                    {
                        product_name = "Melon",
                        product_price = 200,
                        supplier_id = 3,
                        created_by = "John",
                        updated_by = "John"
                    });

                var zl = new ConfigManager<NpgsqlDataAdapter>(connection);

                using (zl.Table(table, "order by product_id").AsTextFile().AsCsvFile())
                {
                    connection.ExecuteUpdate(null, table,
                        new
                        {
                            product_price = 165,
                            updated_at = DateTime.Now,
                            updated_by = "Sam"
                        },
                        new
                        {
                            product_id = 1
                        });
                }
            }
        }

        private const string table = "zengo_test";

        private readonly string ddl = $@"
drop table if exists zengo_test;
create table {table}
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
);
";
    }
}