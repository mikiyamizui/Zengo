using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Zengo.Core;
using Zengo.Interfaces;

namespace Zengo
{
    public class Disposable<TDataAdapter> : IDisposable
        where TDataAdapter : DbDataAdapter
    {
        private IManager<TDataAdapter> _manager;

        private IEnumerable<ITable> _before;
        private IEnumerable<ITable> _after;

        public Disposable(IManager<TDataAdapter> manager)
        {
            _manager = manager;
            _before = _manager.CollectTables();
        }

        public void Dispose()
        {
            _after = _manager.CollectTables();
            _manager.Loggers.ToList().ForEach(logger =>
            {
                logger.Write(_before, _after);
            });
        }
    }
}