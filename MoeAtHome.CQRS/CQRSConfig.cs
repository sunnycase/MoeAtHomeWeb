using MoeAtHome.CQRS.Commands;
using MoeAtHome.CQRS.Queries;
using Owin;
using StructureMap.Configuration.DSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tomato.CQRS;
using Tomato.CQRS.Infrastructure;

namespace MoeAtHome.CQRS
{
    public class CQRSConfig
    {
        public static void ConfigCQRS(IAppBuilder app)
        {
            app.UseCQRS(new MessagingRegistry());
        }
    }

    class MessagingRegistry : Registry
    {
        public MessagingRegistry()
        {
            For<ICommandExecutorFactory>().Use(_ => new CommandExecutorFactory(typeof(MessagingRegistry).Assembly, typeof(IDummyCommand).Namespace));
            For<IQueryExecutorFactory>().Use(_=> new QueryExecutorFactory(typeof(MessagingRegistry).Assembly, typeof(IDummyQuery).Namespace));
        }
    }
}
