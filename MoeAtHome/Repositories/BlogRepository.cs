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
        private BlogAmountRepository blogAmountRepo;

        public BlogRepository(CloudTableClient client)
            :base(client.GetTableReference(Blog.TableName))
        {
            blogAmountRepo = new BlogAmountRepository(client);
        }

        public Blog FindBlog(DateTime date, string title)
        {
            return base.Find(date.ToString(Blog.DateFormat), title);
        }

        public Task<Blog> FindBlogAsync(DateTime date, string title)
        {
            return base.FindAsync(date.ToString(Blog.DateFormat), title);
        }

        public async Task PostBlogAsync(Blog blog)
        {
            await AddAsync(blog);
            await blogAmountRepo.AddAmount(blog.DateTime);
        }

        public IEnumerable<Blog> QueryBlogsDescending(int count)
        {
            var amounts = from b in blogAmountRepo.Query().AsEnumerable()
                          orderby b.Date descending
                          select b;
            var result = new List<Blog>(count);
            var toRead = count;

            foreach (var a in amounts)
            {
                if (toRead > 0)
                {
                    var blogs = from b in Table.CreateQuery<Blog>()
                                where b.PartitionKey == a.PartitionKey
                                select b;
                    result.AddRange(blogs.Take(toRead).AsEnumerable()
                        .OrderByDescending(o => o.DateTime));
                    toRead = count - result.Count;
                }
            }
            return result;
        }
    }
}