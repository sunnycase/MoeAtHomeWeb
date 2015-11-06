using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;

namespace Tomato.CQRS.Core
{
    /// <summary>
    /// 查询执行器工厂
    /// </summary>
    class QueryExecutorFactory : IQueryExecutorFactory
    {
        private readonly Dictionary<Type, Type> _executorsMap;

        public QueryExecutorFactory(Dictionary<Type, Type> executorsMap)
        {
            if (executorsMap == null)
                throw new ArgumentNullException(nameof(executorsMap));

            _executorsMap = executorsMap;
        }

        /// <summary>
        /// 创建查询执行器
        /// </summary>
        /// <param name="queryType">查询类型</param>
        /// <typeparam name="TResult">查询结果类型</typeparam>
        /// <returns>查询执行器</returns>
        public IQueryExecutor<IQuery<TResult>, TResult> Create<TResult>(IServiceProvider serviceProvider, Type queryType)
        {
            Type executorType;
            if (_executorsMap.TryGetValue(queryType, out executorType))
                return (IQueryExecutor<IQuery<TResult>, TResult>)ActivatorUtilities.CreateInstance(serviceProvider, queryType);
            throw new UnregisteredCommandExecutorException(queryType);
        }
    }
}
