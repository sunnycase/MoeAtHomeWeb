using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tomato.CQRS.Core;

namespace Tomato.CQRS
{
    /// <summary>
    /// 查询链
    /// </summary>
    /// <typeparam name="TArgs">参数类型</typeparam>
    /// <typeparam name="TResult">查询结果类型</typeparam>
    public class QueryChain<TArgs, TResult>
    {
        private QueryMiddleware<TArgs, TResult> headMiddleware = null;
        private List<Tuple<Type, object[]>> middlewareParams = new List<Tuple<Type, object[]>>();
        private bool chainUpdated = true;

        /// <summary>
        /// 使用中间件
        /// </summary>
        /// <typeparam name="TMiddleware">中间件类型</typeparam>
        /// <param name="args">参数</param>
        public void Use<TMiddleware>(params object[] args) where TMiddleware : QueryMiddleware<TArgs, TResult>
        {
            middlewareParams.Add(Tuple.Create(typeof(TMiddleware), args));
            chainUpdated = false;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns>查询结果</returns>
        public async Task<TResult> QueryAsync(TArgs args)
        {
            if (!chainUpdated) BuildChain();

            if (headMiddleware != null)
                return await headMiddleware.QueryAsync(args);

            throw new QueryMiddlewareNotFoundException(this.GetType());
        }

        /// <summary>
        /// 创建查询链
        /// </summary>
        public void BuildChain()
        {
            headMiddleware = null;
            var reversedParams = Enumerable.Reverse(middlewareParams);
            QueryMiddleware<TArgs, TResult> lastMiddleware = null;

            foreach (var param in reversedParams)
            {
                var expArgs = CreateActivatorArguments(lastMiddleware, param.Item2);
                var middleware = (QueryMiddleware<TArgs, TResult>)ServiceLocator.Default.IoC.GetInstance(param.Item1, expArgs);
                lastMiddleware = middleware;
            }
            headMiddleware = lastMiddleware;
            chainUpdated = true;
        }

        static ExplicitArguments CreateActivatorArguments(QueryMiddleware<TArgs, TResult> next, object[] args)
        {
            var expArgs = new ExplicitArguments();

            expArgs.Set(next);
            foreach (var arg in args)
                expArgs.Set(arg.GetType(), arg);
            return expArgs;
        }
    }
}
