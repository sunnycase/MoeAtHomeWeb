using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Core
{
    /// <summary>
    /// 命令总线的默认实现
    /// </summary>
    class CommandBus : ICommandBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommandExecutorFactory _executorFactory;

        /// <summary>
        /// 创建命令总线的一个实例
        /// </summary>
        /// <param name="executorFactory">命令执行器工厂</param>
        public CommandBus(IServiceProvider serviceProvider, ICommandExecutorFactory executorFactory)
        {
            _serviceProvider = serviceProvider;
            _executorFactory = executorFactory;
        }

        public Task SendAsync(ICommand command)
        {
            var executor = _executorFactory.Create(_serviceProvider, command.GetType());
            return executor.ExecuteAsync(command);
        }

        public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command)
        {
            await SendAsync((ICommand)command);
            return command.Result;
        }
    }
}
