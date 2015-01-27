using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Infrastructure
{
    /// <summary>
    /// 命令总线的默认实现
    /// </summary>
    class CommandBus : ICommandBus
    {
        private readonly ICommandExecutorFactory executorFactory;

        /// <summary>
        /// 创建命令总线的一个实例
        /// </summary>
        /// <param name="executorFactory">命令执行器工厂</param>
        public CommandBus(ICommandExecutorFactory executorFactory)
        {
            this.executorFactory = executorFactory;
        }

        public Task SendAsync(ICommand command)
        {
            var executor = executorFactory.Create(command.GetType());
            return executor.ExecuteAsync(command);
        }

        public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command)
        {
            var executor = executorFactory.Create(command.GetType());
            await executor.ExecuteAsync(command);
            return command.Result;
        }
    }
}
