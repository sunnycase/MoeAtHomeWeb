using MoeAtHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MoeAtHome.WorkUnits
{
    public interface IBlogCommentWorkUnit
    {
        Task<IEnumerable<ViewModels.Comment>> QueryBlogCommentsDescendingAsync(BlogKey key, 
            long startRowTick, int count);

        Task PostBlogCommentAsync(BlogKey key, string author, DateTime dateTime, string content);
    }
}