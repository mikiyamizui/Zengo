namespace Zengo.Interfaces
{
    public interface IItem
    {
        IColumn Column { get; }
        object Value { get; set; }
        bool IsDBNull { get; }
    }
}