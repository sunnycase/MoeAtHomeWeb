using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoeAtHome.Models
{
    public struct BlogKey
    {
        public string DateTime { get; set; }
        public string Title { get; set; }

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