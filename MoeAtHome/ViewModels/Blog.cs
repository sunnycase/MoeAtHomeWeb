using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoeAtHome.ViewModels
{
    public class Blog
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public List<string> Tags { get; set; }
        public string Content { get; set; }
        public uint ReadersCount { get; set; }
        public uint CommentsCount { get; set; }
    }
}