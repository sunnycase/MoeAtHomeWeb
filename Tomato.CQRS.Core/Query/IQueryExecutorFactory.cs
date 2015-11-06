using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Core
{
    /// <summary>
    /// 查询执行器工厂
    /// </summary>
    public interface IQueryExecutorFactory
    {
        /// <summary>
        /// 创建查询执行器
        /// </summary>
        /// <param name="queryType">查询类型</param>
        /// <typeparam name="TResult">查询结果类型</typeparam>
        /// <returns>查询执行器</returns>
        IQueryExecutor<IQuery<TResult>, TResult> Create<TResult>(IServiceProvider serviceProvider, Type queryType);
    }
}
