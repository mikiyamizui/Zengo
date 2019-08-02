using System;
using System.Data.Common;

namespace Zengo
{
    public interface ILoggerManager<TDataAdapter> : IDisposable
        where TDataAdapter : DbDataAdapter
    {
        ILoggerManager<TDataAdapter> AsPlaneText();
    }
}