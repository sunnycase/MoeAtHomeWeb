using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Core
{
    class QueryBus : IQueryBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IQueryExecutorFactory _executorFactory;

        public QueryBus(IServiceProvider serviceProvider, IQueryExecutorFactory executorFactory)
        {
            _serviceProvider = serviceProvider;
            _executorFactory = executorFactory;
        }

        public Task<TResult> SendAsync<TResult>(IQuery<TResult> query)
        {
            var executor = _executorFactory.Create<TResult>(_serviceProvider, query.GetType());
            return executor.ExecuteAsync(query);
        }
    }
}
