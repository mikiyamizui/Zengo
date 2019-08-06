using System;
using System.Collections.Generic;

namespace Zengo.Interfaces
{
    public interface ITable
    {
        DateTime DateTime { get; }
        string Name { get; }
        string Sql { get; }
        IReadOnlyList<IColumn> Columns { get; }
        IReadOnlyList<IRow> Rows { get; }
    }
}