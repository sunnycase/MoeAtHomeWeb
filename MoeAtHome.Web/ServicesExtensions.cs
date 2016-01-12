using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MoeAtHome.Web.Options;
using MoeAtHome.Web.Services;

namespace MoeAtHome.Web
{
    public static class ServicesExtensions
    {
        public static void AddLyricsService(this IServiceCollection services, Action<LyricsServiceOptions> configureOptions)
        {
            services.AddScoped<LyricsService>();
            services.AddScoped<LyricsStorageService>();
            services.Configure(configureOptions);
        }
    }
}
