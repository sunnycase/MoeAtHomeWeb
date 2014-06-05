using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoeAtHome.Models
{
    public class NestedComment
    {
        public string Author { get; set; }
        public DateTime DateTime { get; set; }
        public string Content { get; set; }
    }
}