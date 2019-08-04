using Zengo.Interfaces;

namespace Zengo.Core
{
    internal class Item : IItem
    {
        public IColumn Column { get; }

        public object Value { get; set; }

        public bool IsDBNull { get; }

        public Item(IColumn column, object value, bool isNull)
        {
            Column = column;
            Value = value;
            IsDBNull = isNull;
        }
    }
}