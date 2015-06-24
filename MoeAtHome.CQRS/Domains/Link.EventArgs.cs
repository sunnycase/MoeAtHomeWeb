using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tomato.CQRS;

namespace MoeAtHome.CQRS.Domains
{
    public class LinkCreatedEventArgs : RoutedEventArgs
    {
        private readonly Link link;
        public Link Link
        {
            get { return link; }
        }

        public LinkCreatedEventArgs(Link link)
        {
            this.link = link;
        }
    }
}
