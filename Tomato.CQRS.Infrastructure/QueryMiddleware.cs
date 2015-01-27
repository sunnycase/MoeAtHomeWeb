using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Infrastructure
{
    /// <summary>
    /// 查询中间件
    /// </summary>
    /// <typeparam name="TArgs">参数类型</typeparam>
    /// <typeparam name="TResult">查询结果类型</typeparam>
    public abstract class QueryMiddleware<TArgs, TResult>
    {
        /// <summary>
        /// 查询链的下一个中间件
        /// </summary>
        protected QueryMiddleware<TArgs, TResult> Next { get; private set; }

        public QueryMiddleware(QueryMiddleware<TArgs, TResult> next)
        {
            Next = next;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="e">参数</param>
        /// <returns>一个包含查询结果的异步操作</returns>
        public abstract Task<TResult> QueryAsync(TArgs e);
    }
}
