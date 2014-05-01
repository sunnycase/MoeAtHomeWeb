using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MoeAtHome.ViewModels
{
    public class PostBlogViewModel
    {
        [Required]
        public string Title { get; set; }

        public List<string> Tags { get; set; }

        [Required]
        public string Content { get; set; }
    }
}