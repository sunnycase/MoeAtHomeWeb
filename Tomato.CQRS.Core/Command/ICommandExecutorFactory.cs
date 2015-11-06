using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Core
{
    /// <summary>
    /// 命令执行器工厂
    /// </summary>
    public interface ICommandExecutorFactory
    {
        /// <summary>
        /// 创建命令执行器
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <returns>一个命令执行器</returns>
        ICommandExecutor<ICommand> Create(IServiceProvider serviceProvider, Type commandType);
    }
}
