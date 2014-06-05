using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MoeAtHome.ViewModels
{
    public class PostBlogCommentViewModel
    {
        [Required]
        public string Content { get; set; }
    }
}