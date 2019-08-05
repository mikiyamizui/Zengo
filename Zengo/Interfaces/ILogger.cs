using System.Collections.Generic;

namespace Zengo.Interfaces
{
    public interface ILogger
    {
        void Write(IEnumerable<ITable> before, IEnumerable<ITable> after);
    }
}