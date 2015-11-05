using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Core
{
    /// <summary>
    /// 查询执行器工厂的默认实现
    /// </summary>
    public class QueryExecutorFactory : IQueryExecutorFactory
    {
        private readonly string executorsDefineNamespace;
        private readonly Assembly executorsDefineAssembly;
        private static ConcurrentDictionary<Type, Type> cachedExecutorTypes = new ConcurrentDictionary<Type, Type>();

        public QueryExecutorFactory(Assembly executorsDefineAssembly, string executorsDefineNamespace)
        {
            this.executorsDefineAssembly = executorsDefineAssembly;
            this.executorsDefineNamespace = executorsDefineNamespace;
        }

        /// <summary>
        /// 创建查询执行器
        /// </summary>
        /// <param name="queryType">查询类型</param>
        /// <typeparam name="TResult">查询结果类型</typeparam>
        /// <returns>查询执行器</returns>
        public IQueryExecutor<IQuery<TResult>, TResult> Create<TResult>(Type queryType)
        {
            var executorType = cachedExecutorTypes.GetOrAdd(queryType, _ =>
            {
                var type = GetQueryExecutorTypes<TResult>(queryType).FirstOrDefault();

                if (type != null)
                    return type;
                else
                    throw new UnregisteredCommandExecutorException(queryType);
            });
            return (IQueryExecutor<IQuery<TResult>, TResult>)ServiceLocator.Default.GetPerSession(executorType);
        }

        protected virtual IEnumerable<Type> GetQueryExecutorTypes<TResult>(Type queryType)
        {
            var executorFaceType = typeof(IQueryExecutor<,>).MakeGenericType(queryType, typeof(TResult));
            var types = from t in executorsDefineAssembly.DefinedTypes
                        where t.IsClass && t.Namespace == executorsDefineNamespace &&
                        t.ImplementedInterfaces.Contains(executorFaceType)
                        select t;

            return types;
        }
    }
}
