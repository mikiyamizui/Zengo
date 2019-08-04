using System.Collections.Generic;

namespace Zengo.Interfaces
{
    public interface IRow
    {
        IReadOnlyList<IItem> Items { get; }
    }
}