using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tomato.CQRS;

namespace MoeAtHome.CQRS.Domains
{
    /// <summary>
    /// 链接
    /// </summary>
    public class Link : AggregateRoot
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// URL
        /// </summary>
        public Uri Url { get; private set; }


        #region 事件

        private RoutedEvent<LinkCreatedEventArgs> createdEvent = new RoutedEvent<LinkCreatedEventArgs>();
        /// <summary>
        /// 创建 Link 后引发
        /// </summary>
        public event RoutedEventHandler<LinkCreatedEventArgs> Created
        {
            add { createdEvent.Add(value); }
            remove { createdEvent.Remove(value); }
        }


        #endregion

        private Link()
        {

        }

        /// <summary>
        /// 创建 Link
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="url">URL</param>
        /// <returns>创建后的 Link</returns>
        public static async Task<Link> Create(string name, Uri url)
        {
            var link = new Link() { Name = name,  Url = url };
            await link.createdEvent.ExecuteChainAsync(new LinkCreatedEventArgs(link));
            return link;
        }
    }
}
