using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Tomato.CQRS.Core
{
    /// <summary>
    /// 命令执行器工厂的
    /// </summary>
    class CommandExecutorFactory : ICommandExecutorFactory
    {
        private readonly Dictionary<Type, Type> _executorsMap;

        public CommandExecutorFactory(Dictionary<Type, Type> executorsMap)
        {
            if (executorsMap == null)
                throw new ArgumentNullException(nameof(executorsMap));
            
            _executorsMap = executorsMap;
        }

        /// <summary>
        /// 创建命令执行器
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <returns>一个命令执行器</returns>
        public ICommandExecutor<ICommand> Create(IServiceProvider serviceProvider, Type commandType)
        {
            Type executorType;
            if (_executorsMap.TryGetValue(commandType, out executorType))
                return (ICommandExecutor<ICommand>)ActivatorUtilities.CreateInstance(serviceProvider, commandType);
            throw new UnregisteredCommandExecutorException(commandType);
        }
    }
}
