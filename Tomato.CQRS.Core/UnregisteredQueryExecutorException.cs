using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Core
{
    /// <summary>
    /// 未注册的查询执行器
    /// </summary>
    public class UnregisteredQueryExecutorException : ApplicationException
    {
        private readonly Type queryType;

        /// <summary>
        /// 查询类型
        /// </summary>
        public Type QueryType { get { return queryType; } }

        public UnregisteredQueryExecutorException(Type queryType, string message)
            : base(message)
        {
            this.queryType = queryType;
        }

        public UnregisteredQueryExecutorException(Type queryType)
            : this(queryType, "未为 " + queryType.ToString() + "类型的查询注册查询执行器。")
        {

        }
    }
}
