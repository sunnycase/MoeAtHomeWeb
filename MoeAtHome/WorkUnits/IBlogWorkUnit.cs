using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoeAtHome.WorkUnits
{
    public interface IBlogWorkUnit
    {
        Task<Models.Blog> FindBlogAsync(DateTime date, string title);
        Task PostBlogAsync(Models.Blog blog);
        Task<IEnumerable<ViewModels.Blog>> QueryRecentsBlogsPrevewAsync(int count);
    }
}
