using System;
using System.IO;

namespace Zengo.Interfaces
{
    public interface IZengoLogger
    {
        string Extension { get; }

        void OnWrite(TextWriter sw,
            (string ColumnName, Type DataType, int Length)[] columns,
            (int Ordinal, object Value, bool IsDBNull)[][] rows);
    }
}