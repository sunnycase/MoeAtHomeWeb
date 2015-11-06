using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tomato.CQRS.Core;

namespace Tomato.CQRS.Builder
{
    class CommandExecutorFactoryBuilder : ICommandExecutorFactoryBuilder
    {
        private readonly Dictionary<Type, Type> _executorsMap = new Dictionary<Type, Type>();
        
        public ICommandExecutorFactory BuildUp()
        {
            return new CommandExecutorFactory(_executorsMap);
        }

        ICommandExecutorFactoryBuilder ICommandExecutorFactoryBuilder.Use<TCommand, TCommandExecutor>()
        {
            _executorsMap[typeof(TCommand)] = typeof(TCommandExecutor);
            return this;
        }
    }
}
