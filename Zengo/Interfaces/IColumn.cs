using System;

namespace Zengo.Interfaces
{
    public interface IColumn
    {
        int Index { get; }
        string Name { get; }
        Type DataType { get; }
    }
}