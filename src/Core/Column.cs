using System;
using Zengo.Interfaces;

namespace Zengo.Core
{
    internal class Column : IColumn
    {
        public int Index { get; }

        public string Name { get; }

        public Type DataType { get; }

        public Column(int index, string name, Type dataType)
        {
            Index = index;
            Name = name;
            DataType = dataType;
        }
    }
}
