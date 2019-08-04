using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Zengo.Core;
using Zengo.Interfaces;

namespace Zengo
{
    public interface IManager<TDataAdapter>
        where TDataAdapter : DbDataAdapter
    {
        IDictionary<string, string> Tables { get; }

        IList<ILogger> Loggers { get; }

        IEnumerable<ITable> CollectTables();

        Disposable<TDataAdapter> ToDisposable();
    }
}