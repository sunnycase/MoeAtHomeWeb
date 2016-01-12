using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoeAtHome.Web.Options
{
    public class LyricsServiceOptions
    {
        public string DbConnectionString { get; set; }
        public string StorageDirectory { get; set; }
    }
}
