using MoeAtHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MoeAtHome.Repositories
{
    public interface IBlogCommentRepository : IRepository<BlogComment>
    {
        Task<IQueryable<BlogComment>> QueryBlogCommentsDescendingAsync(BlogKey key, 
            long startRowTick, int count);
    }
}