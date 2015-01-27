using StructureMap.Configuration.DSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.CQRS.Infrastructure
{
    public class InfrastructureRegistry : Registry
    {
        public InfrastructureRegistry()
        {
            For<ICommandBus>().Singleton().Use<CommandBus>();
            For<IQueryBus>().Singleton().Use<QueryBus>();
        }
    }
}
