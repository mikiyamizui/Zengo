using System;
using System.Data;

namespace Zengo
{
    public interface ILoggerManager<TDataAdapter> : IDisposable
        where TDataAdapter : IDbDataAdapter
    {
        ILoggerManager<TDataAdapter> AsPlaneText();
    }
}