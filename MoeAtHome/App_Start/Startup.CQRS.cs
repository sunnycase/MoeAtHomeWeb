using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MoeAtHome.CQRS;

namespace MoeAtHome
{
	public partial class Startup
	{
        public void ConfigCQRS(IAppBuilder app)
        {
            CQRSConfig.ConfigCQRS(app);
        }
	}
}