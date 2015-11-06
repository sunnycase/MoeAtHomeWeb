using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tomato.CQRS.Core;

namespace Tomato.CQRS.Builder
{
    public interface ICommandExecutorFactoryBuilder
    {
        ICommandExecutorFactoryBuilder Use<TCommand, TCommandExecutor>() where TCommand : class, ICommand where TCommandExecutor : class, ICommandExecutor<TCommand>;
    }
}
