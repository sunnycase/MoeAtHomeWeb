using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;

namespace Tomato.CQRS.Builder
{
    class CQRSBuilder : ICQRSBuilder
    {
        public IServiceCollection Services { get; }

        public CQRSBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
