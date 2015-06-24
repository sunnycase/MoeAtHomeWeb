using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tomato.CQRS.Infrastructure;

namespace Tomato.CQRS
{
    /// <summary>
    /// 路由事件参数
    /// </summary>
    public class RoutedEventArgs : EventArgs
    {
        /// <summary>
        /// 表示事件已被处理
        /// </summary>
        public bool IsHandled { get; set; } = false;
    }

    /// <summary>
    /// 表示处理包含参数的路由事件的方法
    /// </summary>
    /// <typeparam name="TEventArgs">参数类型</typeparam>
    /// <param name="e">参数</param>
    /// <returns>一个异步操作</returns>
    public delegate Task RoutedEventHandler<in TEventArgs>(TEventArgs e) where TEventArgs : RoutedEventArgs;

    /// <summary>
    /// 路由事件
    /// </summary>
    /// <typeparam name="TEventArgs">参数类型</typeparam>
    public class RoutedEvent<TEventArgs> where TEventArgs : RoutedEventArgs
    {
        private List<RoutedEventHandler<TEventArgs>> _handlers =
            new List<RoutedEventHandler<TEventArgs>>();

        /// <summary>
        /// 添加处理方法
        /// </summary>
        /// <param name="handler">处理方法</param>
        public void Add(RoutedEventHandler<TEventArgs> handler)
        {
            _handlers.Add(handler);
        }

        public void Remove(RoutedEventHandler<TEventArgs> handler)
        {
            _handlers.Remove(handler);
        }

        /// <summary>
        /// 链式执行处理方法
        /// </summary>
        /// <param name="e">参数</param>
        public async Task ExecuteChainAsync(TEventArgs e)
        {
            foreach (var handler in _handlers)
            {
                if (e.IsHandled) break;
                await handler(e);
            }
        }
    }
}
