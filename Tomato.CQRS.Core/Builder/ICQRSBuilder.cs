using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;

namespace Tomato.CQRS.Builder
{
    public interface ICQRSBuilder
    {
        IServiceCollection Services { get; }
    }
}
