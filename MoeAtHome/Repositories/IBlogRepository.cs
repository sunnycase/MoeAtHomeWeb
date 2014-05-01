using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoeAtHome.Repositories
{
    public interface IBlogRepository : IRepository<Models.Blog>
    {
        IEnumerable<Models.Blog> QueryBlogsDescending(int count);
    }
}
