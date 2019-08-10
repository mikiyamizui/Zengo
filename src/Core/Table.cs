using System;
using System.Collections.Generic;
using System.Linq;
using Zengo.Interfaces;

namespace Zengo
{
    internal class Table : ITable
    {
        public DateTime DateTime { get; }

        public string Name { get; }

        public string Sql { get; }

        public IReadOnlyList<IColumn> Columns { get; }

        public IReadOnlyList<IRow> Rows { get; }

        public Table(string name, string sql, IEnumerable<IColumn> columns, IEnumerable<IRow> rows, bool autoSort)
        {
            Name = name;
            Sql = sql;
            Columns = columns.ToList();

            if (autoSort && Columns.Any())
            {
                var ordered = rows.OrderBy(row => row.Items.First().Value);
                rows = Columns.Skip(1).Aggregate(ordered, (o, c) => o.ThenBy(row => row.Items[c.Index].Value));

                /*
                Enumerable.Range(0, Columns.Count).Skip(1).ToList()
                    .ForEach(i => ordered = ordered.ThenBy(row => row.Items[i].Value));
                rows = ordered;
                */
            }

            Rows = rows.ToList();
            DateTime = DateTime.Now;
        }
    }
}
