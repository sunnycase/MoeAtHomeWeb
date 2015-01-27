using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Infrastructure
{
    /// <summary>
    /// 命令执行器工厂的默认实现
    /// </summary>
    public class CommandExecutorFactory : ICommandExecutorFactory
    {
        private readonly string executorsDefineNamespace;
        private readonly Assembly executorsDefineAssembly;
        private Dictionary<Type, Type> cachedExecutorTypes = new Dictionary<Type, Type>();

        public CommandExecutorFactory(Assembly executorsDefineAssembly, string executorsDefineNamespace)
        {
            this.executorsDefineAssembly = executorsDefineAssembly;
            this.executorsDefineNamespace = executorsDefineNamespace;
        }

        /// <summary>
        /// 创建命令执行器
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <returns>一个命令执行器</returns>
        public ICommandExecutor<ICommand> Create(Type commandType)
        {
            Type executorType;
            if (!cachedExecutorTypes.TryGetValue(commandType, out executorType))
            {
                executorType = GetCommandExecutorTypes(commandType).FirstOrDefault();

                if (executorType != null)
                    cachedExecutorTypes.Add(commandType, executorType);
                else
                    throw new UnregisteredCommandExecutorException(commandType);
            }
            return (ICommandExecutor<ICommand>)ServiceLocator.Default.GetPerSession(executorType);
        }

        protected virtual IEnumerable<Type> GetCommandExecutorTypes(Type commandType)
        {
            var executorFaceType = typeof(ICommandExecutor<>).MakeGenericType(commandType);
            var types = from t in executorsDefineAssembly.DefinedTypes
                        where t.IsClass && t.Namespace == executorsDefineNamespace &&
                        t.ImplementedInterfaces.Contains(executorFaceType)
                        select t;

            return types;
        }
    }
}
