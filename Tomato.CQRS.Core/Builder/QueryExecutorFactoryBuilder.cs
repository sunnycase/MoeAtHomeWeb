using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tomato.CQRS.Core;

namespace Tomato.CQRS.Builder
{
    class QueryExecutorFactoryBuilder : IQueryExecutorFactoryBuilder
    {
        private readonly Dictionary<Type, Type> _executorsMap = new Dictionary<Type, Type>();

        public IQueryExecutorFactory BuildUp()
        {
            return new QueryExecutorFactory(_executorsMap);
        }

        IQueryExecutorFactoryBuilder IQueryExecutorFactoryBuilder.Use<TQuery, TQueryExecutor, TResult>()
        {
            _executorsMap[typeof(TQuery)] = typeof(TQueryExecutor);
            return this;
        }
    }
}
