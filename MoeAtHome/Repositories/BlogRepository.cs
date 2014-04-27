using Microsoft.WindowsAzure.Storage.Table;
using MoeAtHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MoeAtHome.Repositories
{
    public class BlogRepository : Repository<Blog>, IBlogRepository
    {
        public BlogRepository(CloudTableClient client)
            :base(client.GetTableReference(Blog.TableName))
        {
        }

        public Blog FindBlog(DateTime date, string title)
        {
            return base.Find(date.ToString(Blog.DateFormat), title);
        }

        public Task<Blog> FindBlogAsync(DateTime date, string title)
        {
            return base.FindAsync(date.ToString(Blog.DateFormat), title);
        }
    }
}