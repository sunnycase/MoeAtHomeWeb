using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Core
{
    class QueryBus : IQueryBus
    {
        private readonly IQueryExecutorFactory executorFactory;

        public QueryBus(IQueryExecutorFactory executorFactory)
        {
            this.executorFactory = executorFactory;
        }

        public Task<TResult> SendAsync<TResult>(IQuery<TResult> query)
        {
            var executor = executorFactory.Create<TResult>(query.GetType());
            return executor.ExecuteAsync(query);
        }
    }
}
