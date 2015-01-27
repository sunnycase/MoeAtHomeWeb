using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS
{
    /// <summary>
    /// 命令
    /// </summary>
    public interface ICommand
    {
    }

    /// <summary>
    /// 返回执行结果的命令
    /// </summary>
    /// <typeparam name="TResult">执行结果类型</typeparam>
    public interface ICommand<out TResult> : ICommand
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        TResult Result { get; }
    }
}
