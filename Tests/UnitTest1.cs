using System;
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
        private const string table = "zengo_test";
        private const string filter = "";//"order by product_id";

        [TestMethod]
        public void TestMethod1()
        {
            using (var connection = new NpgsqlConnection("Server=localhost;Port=5432;Database=test;UserId=postgres;Password=postgres;Enlist=true;"))
            {
                connection.Open();

                SetupTestData(connection);

                using (connection.Zengo()
                    .AsCsv(table, filter)
                    .AsExcel(table, filter)
                    .AsJson(table, filter)
                    .ToDisposable())
                {
                    Task.Delay(TimeSpan.FromSeconds(3)).Wait();

                    connection.Execute($@"update {table} set product_price = @product_price, updated_at = now(), updated_by = @updated_by where product_id = @product_id",
                        new { product_id = 1, product_price = 165, updated_by = "Sam" });
                }
            }
        }

        private void SetupTestData(NpgsqlConnection connection)
        {
            connection.Execute($@"
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
");

            connection.Execute($@"insert into {table} (product_name, product_price, supplier_id, created_by, updated_by) values (@product_name, @product_price, @supplier_id, @created_by, @updated_by)",
                new[]
                {
                    new { product_name = "Apple", product_price = 100, supplier_id = 1, created_by = "John", updated_by = "John" },
                    new { product_name = "Orange", product_price = 150, supplier_id = 2, created_by = "John", updated_by = "John" },
                    new { product_name = "Melon", product_price = 200, supplier_id = 3, created_by = "John", updated_by = "John" }
                });
        }
    }
}