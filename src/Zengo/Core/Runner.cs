using System;
using System.Collections.Generic;
using System.Linq;
using Zengo.Interfaces;

namespace Zengo.Core
{
    public class Disposable : IDisposable
    {
        private readonly IBuilder _builder;
        private readonly IEnumerable<ITable> _before;
        private IEnumerable<ITable> _after;

        public Disposable(IBuilder manager)
        {
            _builder = manager;
            _before = _builder.CollectTables();
        }

        public void Dispose()
        {
            _after = _builder.CollectTables();
            _builder.Loggers.ToList().ForEach(logger =>
            {
                logger.Write(_before, _after);
            });
        }
    }
}