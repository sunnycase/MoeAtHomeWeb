using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MoeAtHome.Web.Infrastructure
{
    public class Friend
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
    }
}
