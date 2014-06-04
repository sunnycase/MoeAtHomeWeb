using Microsoft.WindowsAzure.Storage.Table;
using MoeAtHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace MoeAtHome.Repositories
{
    public class BlogRepository : Repository<Blog>, IBlogRepository
    {
        private IBlogAmountRepository blogAmountRepo;

        public BlogRepository(CloudTableClient client)
            : base(client.GetTableReference(Blog.TableName))
        {
            blogAmountRepo = new BlogAmountRepository(client);
        }

        public Task<Blog> FindBlogAsync(DateTime date, string title)
        {
            return base.FindAsync(date.ToString(Blog.DateFormat), title);
        }

        public async Task PostBlogAsync(Blog blog)
        {
            await AddAsync(blog);
            await blogAmountRepo.AddAmountAsync(blog.DateTime);
        }

        public async Task<IEnumerable<ViewModels.Blog>> QueryRecentsBlogsPrevewAsync(int count)
        {
            var amounts = from b in blogAmountRepo.Query().AsEnumerable()
                          orderby b.Date descending
                          select b;
            var result = new List<ViewModels.Blog>(count);
            var toRead = count;

            foreach (var a in amounts)
            {
                var thePass = Math.Min(toRead, a.Amount);
                var blogs = from b in Table.CreateQuery<Blog>()
                            where b.PartitionKey == a.PartitionKey
                            select new
                            {
                                Date = b.DateString,
                                DateTime = b.DateTime,
                                Title = b.Title,
                                Tags = b.SerializedTags,
                                Summary = b.Content.Substring(0, Math.Min(b.Content.Length, 200)),
                                ReadersCount = b.ReadersCount,
                                CommentsCount = b.CommentsCount
                            };

                result.AddRange(blogs.AsEnumerable().OrderByDescending(o => o.DateTime).Take(thePass)
                    .Select(o => new ViewModels.Blog
                    {
                        Date = o.Date,
                        DateTime = o.DateTime,
                        Title = o.Title,
                        Summary = o.Summary,
                        ReadersCount = o.ReadersCount,
                        CommentsCount = o.CommentsCount,
                        Tags = Blog.GetTags(o.Tags),
                    }));
                toRead -= thePass;

                if (toRead == 0)
                    break;
            }
            return result;
        }
    }
}