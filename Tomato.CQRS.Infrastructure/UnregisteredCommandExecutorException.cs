using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Infrastructure
{
    /// <summary>
    /// 未注册的命令执行器
    /// </summary>
    public class UnregisteredCommandExecutorException : ApplicationException
    {
        private readonly Type commandType;

        /// <summary>
        /// 命令类型
        /// </summary>
        public Type CommandType { get { return commandType; } }

        public UnregisteredCommandExecutorException(Type commandType, string message)
            : base(message)
        {
            this.commandType = commandType;
        }

        public UnregisteredCommandExecutorException(Type commandType)
            : this(commandType, "未为 " + commandType.ToString() + "类型的命令注册命令执行器。")
        {

        }
    }
}
