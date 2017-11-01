using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

namespace MoeAtHome.Web.Controllers
{
    internal class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HomeController>();
            builder.RegisterType<FriendController>();
        }
    }
}
