using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Infrastructure
{
    /// <summary>
    /// 查询总线
    /// </summary>
    public interface IQueryBus
    {
        /// <summary>
        /// 发送返回执行结果的查询
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="query">命令</param>
        /// <returns>一个包含执行结果的异步操作</returns>
        Task<TResult> SendAsync<TResult>(IQuery<TResult> query);
    }
}
