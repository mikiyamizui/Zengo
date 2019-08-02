using System;
using System.Data;
using System.Data.Common;

namespace Zengo
{
    public interface ILoggerManager<TDataAdapter> : IDisposable
        where TDataAdapter : IDbDataAdapter
    {
        ILoggerManager<TDataAdapter> AsPlaneText();
    }
}