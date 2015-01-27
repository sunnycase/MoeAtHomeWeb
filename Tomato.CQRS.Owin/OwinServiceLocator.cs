using Microsoft.Owin;
using StructureMap;
using StructureMap.Configuration.DSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Owin
{
    class OwinServiceLocator : ServiceLocator
    {
        private IContainer ioc;
        public override IContainer IoC
        {
            get { return ioc; }
        }

        internal IContainer perSessionIoc;
        public override IContainer PerSessionIoC
        {
            get { return perSessionIoc; }
        }

        public OwinServiceLocator()
        {
            ioc = new Container();
        }

        public OwinServiceLocator(IEnumerable<Registry> registries)
        {
            ioc = new Container(o =>
            {
                foreach (var registry in registries)
                {
                    o.AddRegistry(registry);
                }
            });
        }
    }
}
