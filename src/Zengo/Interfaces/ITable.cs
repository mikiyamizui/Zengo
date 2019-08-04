using System;
using System.Collections.Generic;
using System.Text;

namespace Zengo.Interfaces
{
    public interface ITable
    {
        string Name { get; }
        string Sql { get; }
        IReadOnlyList<IColumn> Columns { get; }
        IReadOnlyList<IRow> Rows { get; }
        DateTime DateTime { get; }
    }
}