using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Infrastructure
{
    /// <summary>
    /// 命令执行器
    /// </summary>
    public interface ICommandExecutor<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <returns>一个异步操作</returns>
        Task ExecuteAsync(TCommand command);
    }
}
