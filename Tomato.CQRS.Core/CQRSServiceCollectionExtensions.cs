using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tomato.CQRS;
using Tomato.CQRS.Builder;
using Tomato.CQRS.Core;

namespace Microsoft.Framework.DependencyInjection
{
    public static class CQRSServiceCollectionExtensions
    {
        public static ICQRSBuilder AddCQRS(this IServiceCollection services)
        {
            return new CQRSBuilder(services);
        }
    }

    public static class CQRSCoreBuilderExtensions
    {
        public static ICQRSBuilder AddCommandBus(this ICQRSBuilder builder, Action<ICommandExecutorFactoryBuilder> configureExecutorFactory = null)
        {
            var executorFactoryBuilder = new CommandExecutorFactoryBuilder();
            configureExecutorFactory?.Invoke(executorFactoryBuilder);
            var services = builder.Services;

            services.AddInstance<ICommandExecutorFactory>(executorFactoryBuilder.BuildUp());
            services.AddScoped<ICommandBus, CommandBus>();
            return builder;
        }

        public static ICQRSBuilder AddQueryBus(this ICQRSBuilder builder, Action<IQueryExecutorFactoryBuilder> configureExecutorFactory = null)
        {
            var executorFactoryBuilder = new QueryExecutorFactoryBuilder();
            configureExecutorFactory?.Invoke(executorFactoryBuilder);
            var services = builder.Services;

            services.AddInstance<IQueryExecutorFactory>(executorFactoryBuilder.BuildUp());
            services.AddScoped<IQueryBus, QueryBus>();
            return builder;
        }
    }
}
