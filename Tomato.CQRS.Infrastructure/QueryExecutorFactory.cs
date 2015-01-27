using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Infrastructure
{
    /// <summary>
    /// 查询执行器工厂的默认实现
    /// </summary>
    public class QueryExecutorFactory : IQueryExecutorFactory
    {
        private readonly string executorsDefineNamespace;
        private readonly Assembly executorsDefineAssembly;
        private Dictionary<Type, Type> cachedExecutorTypes = new Dictionary<Type, Type>();

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
            Type executorType;
            if (!cachedExecutorTypes.TryGetValue(queryType, out executorType))
            {
                executorType = GetQueryExecutorTypes(queryType, typeof(TResult)).FirstOrDefault();

                if (executorType != null)
                    cachedExecutorTypes.Add(queryType, executorType);
                else
                    throw new UnregisteredQueryExecutorException(queryType);
            }
            return (IQueryExecutor<IQuery<TResult>, TResult>)ServiceLocator.Default.GetPerSession(executorType);
        }

        protected virtual IEnumerable<Type> GetQueryExecutorTypes(Type queryType, Type resultType)
        {
            var executorFaceType = typeof(IQueryExecutor<,>).MakeGenericType(queryType, resultType);
            var types = from t in executorsDefineAssembly.DefinedTypes
                        where t.IsClass && t.Namespace == executorsDefineNamespace &&
                        t.ImplementedInterfaces.Contains(executorFaceType)
                        select t;

            return types;
        }
    }
}
