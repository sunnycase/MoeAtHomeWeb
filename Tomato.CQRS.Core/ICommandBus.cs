using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS
{
    /// <summary>
    /// 命令总线
    /// </summary>
    public interface ICommandBus
    {
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <returns>一个异步操作</returns>
        Task SendAsync(ICommand command);

        /// <summary>
        /// 发送返回执行结果的命令
        /// </summary>
        /// <typeparam name="TCommand">命令类型</typeparam>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="command">命令</param>
        /// <param name="dummy">无意义</param>
        /// <returns>一个包含执行结果的异步操作</returns>
        Task<TResult> SendAsync<TResult>(ICommand<TResult> command);
    }
}
