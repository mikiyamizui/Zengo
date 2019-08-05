using System.Collections.Generic;
using System.Linq;
using Zengo.Interfaces;

namespace Zengo.Core
{
    internal class Row : IRow
    {
        public IReadOnlyList<IItem> Items { get; }

        public Row(IEnumerable<IItem> items)
        {
            Items = items.ToList();
        }
    }
}