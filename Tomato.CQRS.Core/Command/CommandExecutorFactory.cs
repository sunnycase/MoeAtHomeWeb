using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using Tomato.CQRS.Core.Configuration;

namespace Tomato.CQRS.Core
{
    /// <summary>
    /// 命令执行器工厂的默认实现
    /// </summary>
    public class CommandExecutorFactory : ICommandExecutorFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _executorsDefineNamespace;
        private readonly Assembly _executorsDefineAssembly;
        private static ConcurrentDictionary<Type, Type> cachedExecutorTypes = new ConcurrentDictionary<Type, Type>();

        public CommandExecutorFactory(IOptions<CommandExecutorFactoryOptions> options, IServiceProvider serviceProvider)
        {
            if (options.Value == null)
                throw new ArgumentNullException(nameof(options));
            if (options.Value.ExecutorsDefineAssembly == null)
                throw new ArgumentNullException(nameof(CommandExecutorFactoryOptions.ExecutorsDefineAssembly));
            if (options.Value.ExecutorsDefineNamespace == null)
                throw new ArgumentNullException(nameof(CommandExecutorFactoryOptions.ExecutorsDefineNamespace));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
            _executorsDefineAssembly = options.Value.ExecutorsDefineAssembly;
            _executorsDefineNamespace = options.Value.ExecutorsDefineNamespace;
        }

        /// <summary>
        /// 创建命令执行器
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <returns>一个命令执行器</returns>
        public ICommandExecutor<ICommand> Create(Type commandType)
        {
            var executorType = cachedExecutorTypes.GetOrAdd(commandType, _ =>
            {
                var type = GetCommandExecutorTypes(commandType).FirstOrDefault();

                if (type != null)
                    return type;
                else
                    throw new UnregisteredCommandExecutorException(commandType);
            });

            return (ICommandExecutor<ICommand>)ActivatorUtilities.CreateInstance(_serviceProvider, executorType);
        }

        protected virtual IEnumerable<Type> GetCommandExecutorTypes(Type commandType)
        {
            var executorFaceType = typeof(ICommandExecutor<>).MakeGenericType(commandType);
            var types = from t in _executorsDefineAssembly.DefinedTypes
                        where t.IsClass && t.Namespace == _executorsDefineNamespace &&
                        t.ImplementedInterfaces.Contains(executorFaceType)
                        select t.AsType();

            return types;
        }
    }
}
