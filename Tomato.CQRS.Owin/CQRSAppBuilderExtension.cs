using Owin;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin
{
    using StructureMap.Configuration.DSL;
    using Tomato.CQRS;
    using Tomato.CQRS.Infrastructure;
    using Tomato.CQRS.Owin;

    public static class CQRSAppBuilderExtension
    {
        /// <summary>
        /// 使用 CQRS 中间件
        /// </summary>
        /// <param name="builder">应用程序</param>
        /// <param name="registries">配置</param>
        public static void UseCQRS(this IAppBuilder builder, params Registry[] registries)
        {
            var serviceLocator = new OwinServiceLocator(new[] { new InfrastructureRegistry() }.Concat(registries));

            builder.Use<CQRSOwinMiddleware>(serviceLocator);
            ServiceLocator.Default = serviceLocator;
        }
    }
}