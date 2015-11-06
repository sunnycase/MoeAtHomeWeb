using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;
using Tomato.CQRS.Core;

namespace Tomato.CQRS.Core
{
    /// <summary>
    /// 查询链
    /// </summary>
    /// <typeparam name="TArgs">参数类型</typeparam>
    /// <typeparam name="TResult">查询结果类型</typeparam>
    public class QueryChain<TArgs, TResult>
    {
        private Lazy<QueryMiddleware<TArgs, TResult>> _headMiddleware;
        private readonly List<Tuple<Type, object[]>> _middlewareParams = new List<Tuple<Type, object[]>>();
        private readonly IServiceProvider _serviceProvider;

        public QueryChain(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 使用中间件
        /// </summary>
        /// <typeparam name="TMiddleware">中间件类型</typeparam>
        /// <param name="args">参数</param>
        public void Use<TMiddleware>(params object[] args) where TMiddleware : QueryMiddleware<TArgs, TResult>
        {
            lock(_middlewareParams)
                _middlewareParams.Add(Tuple.Create(typeof(TMiddleware), args));
            Refresh();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns>查询结果</returns>
        public async Task<TResult> QueryAsync(TArgs args)
        {
            var headMiddleware = _headMiddleware.Value;
            if (headMiddleware != null)
                return await headMiddleware.QueryAsync(args);

            throw new QueryMiddlewareNotFoundException(this.GetType());
        }

        /// <summary>
        /// 创建查询链
        /// </summary>
        private QueryMiddleware<TArgs, TResult> BuildUp()
        {
            List<Tuple<Type, object[]>> reversedParams;
            lock(_middlewareParams)
                reversedParams = Enumerable.Reverse(_middlewareParams).ToList();

            QueryMiddleware<TArgs, TResult> lastMiddleware = null;

            foreach (var param in reversedParams)
                lastMiddleware = (QueryMiddleware<TArgs, TResult>)ActivatorUtilities.CreateInstance(_serviceProvider, param.Item1, lastMiddleware, param.Item2);
            return lastMiddleware;
        }

        private void Refresh()
        {
            _headMiddleware = new Lazy<QueryMiddleware<TArgs, TResult>>(BuildUp, LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
