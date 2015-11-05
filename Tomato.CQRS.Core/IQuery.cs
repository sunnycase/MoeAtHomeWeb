using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS
{
    /// <summary>
    /// 查询
    /// </summary>
    /// <typeparam name="TResult">查询结果类型</typeparam>
    public interface IQuery<out TResult>
    {
    }
}
