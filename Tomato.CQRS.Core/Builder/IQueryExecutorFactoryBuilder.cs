using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tomato.CQRS.Core;

namespace Tomato.CQRS.Builder
{
    public interface IQueryExecutorFactoryBuilder
    {
        IQueryExecutorFactoryBuilder Use<TQuery, TQueryExecutor, TResult>() where TQuery : class, IQuery<TResult> where TQueryExecutor : class, IQueryExecutor<TQuery, TResult>;
    }
}
