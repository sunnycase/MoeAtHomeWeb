using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoeAtHome.Repositories
{
    public interface IBlogRepository : IRepository<Models.Blog>
    {
        Task<Models.Blog> FindBlogAsync(DateTime date, string title);
        Task PostBlogAsync(Models.Blog blog);
        Task<IEnumerable<ViewModels.Blog>> QueryRecentsBlogsPrevewAsync(int count);
    }
}
