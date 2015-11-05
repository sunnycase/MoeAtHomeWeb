using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Core
{
    /// <summary>
    /// 查询执行器
    /// </summary>
    public interface IQueryExecutor<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="query">查询</param>
        /// <returns>查询结果</returns>
        Task<TResult> ExecuteAsync(TQuery query);
    }
}
