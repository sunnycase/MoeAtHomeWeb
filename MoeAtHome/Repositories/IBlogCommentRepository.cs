using MoeAtHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoeAtHome.Repositories
{
    public interface IBlogCommentRepository : IRepository<BlogComment>
    {
        IEnumerable<BlogComment> QueryBlogCommentsDescending(BlogKey key, int count);
    }
}