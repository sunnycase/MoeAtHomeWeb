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
        public List<NestedComment> NestedComments { get; set; }
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static BlogKey Deserialize(string value)
        {
            return JsonConvert.DeserializeObject<BlogKey>(value);
        }
    }
}