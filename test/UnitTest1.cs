using System;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using Zengo;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        private const string TableName = "zengo_test";

        [TestMethod]
        public void TestMethod1()
        {
            Zengo.Config.Csv.Encoding = Encoding.GetEncoding(932);

            using (_connection.Zengo()
                .SaveAsCsv(TableName)
                .SaveAsExcel(TableName)
                .SaveAsJson(TableName)
                .ToDisposable())
            {
                Task.Delay(TimeSpan.FromSeconds(3)).Wait();

                var sql = $@"
update {TableName} set
    product_price = @product_price,
    updated_at = now(),
    updated_by = @updated_by
where product_id = @product_id
";

                _connection.Execute(sql, new
                {
                    product_id = 1,
                    product_price = 165,
                    updated_by = "Sam"
                });
            }
        }

        private static NpgsqlConnection _connection;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _connection?.Dispose();
            _connection = null;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var ddl = $@"
drop table if exists zengo_test;
create table {TableName}
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
            _connection.Execute(ddl);

            var sql = $@"
insert into {TableName} (
    product_name,
    product_price,
    supplier_id,
    created_by,
    updated_by
) values (
    @product_name,
    @product_price,
    @supplier_id,
    @created_by,
    @updated_by
)";
            _connection.Execute(sql, new[]
            {
                new {
                    product_name = "Apple",
                    product_price = 100,
                    supplier_id = 1,
                    created_by = "John",
                    updated_by = "John"
                },
                new {
                    product_name = "Orange",
                    product_price = 150,
                    supplier_id = 2,
                    created_by = "John",
                    updated_by = "John"
                },
                new {
                    product_name = "Melon",
                    product_price = 200,
                    supplier_id = 3,
                    created_by = "John",
                    updated_by = "John"
                }
            });
        }
    }
}