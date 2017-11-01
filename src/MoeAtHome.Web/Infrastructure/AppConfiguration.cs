using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace MoeAtHome.Web.Infrastructure
{
    public class AppSecretConfiguration : IOptions<AppSecretConfiguration>
    {
        public string DbConnectionString { get; set; }

        AppSecretConfiguration IOptions<AppSecretConfiguration>.Value => this;
    }
}
