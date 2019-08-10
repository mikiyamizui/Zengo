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
                IOrderedEnumerable<IRow> ordered = null;

                for (int i = 0; i < Columns.Count; i++)
                {
                    ordered = i == 0
                        ? ordered.OrderBy(row => row.Items[i].Value)
                        : ordered.ThenBy(row => row.Items[i].Value);
                }

                rows = ordered;
            }

            Rows = rows.ToList();
            DateTime = DateTime.Now;
        }
    }
}