using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoeAtHome.Web.Models
{
    public class Link
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Uri Url { get; set; }
    }
}
