using System;
using System.Collections.Generic;
using Zengo.Interfaces;

namespace Zengo
{
    public interface IBuilder
    {
        IDictionary<string, string> Tables { get; }

        IList<ILogger> Loggers { get; }

        IEnumerable<ITable> CollectTables();

        IDisposable ToDisposable();
    }
}