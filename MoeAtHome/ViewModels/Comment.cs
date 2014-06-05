using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoeAtHome.ViewModels
{
    public class Comment
    {
        public DateTime DateTime { get; set; }
        public string AuthorName { get; set; }
        public int Floor { get; set; }
        public string Content { get; set; }
        public IEnumerable<Comment> NestedComments { get; set; }
    }
}