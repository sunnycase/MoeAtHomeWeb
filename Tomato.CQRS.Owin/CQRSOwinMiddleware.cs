using Microsoft.Owin;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Owin
{
    class CQRSOwinMiddleware : OwinMiddleware
    {
        private readonly OwinServiceLocator serviceLocator;

        public CQRSOwinMiddleware(OwinMiddleware next, OwinServiceLocator serviceLocator)
            :base(next)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException(nameof(serviceLocator));
            this.serviceLocator = serviceLocator;
        }

        public override async Task Invoke(IOwinContext context)
        {
            using (var nestedIoC = serviceLocator.IoC.GetNestedContainer())
            {
                serviceLocator.perSessionIoc = nestedIoC;
                context.Set(Constants.PerSessionIoCKey, nestedIoC);
                await Next.Invoke(context);
                context.Set<IContainer>(Constants.PerSessionIoCKey, null);
                serviceLocator.perSessionIoc = null;
            }
        }
    }
}

namespace Microsoft.Owin
{
    using Tomato.CQRS.Owin;

    public static class CQRSOwinExtension
    {
        public static IContainer GetPerSessionIoC(this IOwinContext context)
        {
            return context.Get<IContainer>(Constants.PerSessionIoCKey);
        }
    }
}