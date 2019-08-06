﻿using System;
using System.Collections.Generic;
using System.Data;
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

        public Table(string name, string sql, IEnumerable<IColumn> columns, IEnumerable<IRow> rows, bool sort)
        {
            Name = name;
            Sql = sql;
            Columns = columns.ToList();

            if (sort)
            {
                rows = rows.OrderBy(row => row.Items.Select(item => item.Value).FirstOrDefault());
            }
            Rows = rows.ToList();

            DateTime = DateTime.Now;
        }
    }
}