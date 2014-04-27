using MoeAtHome.ViewModels;
using MoeAtHome.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MoeAtHome.Controllers
{
    [RoutePrefix("api/blog")]
    public class BlogController : ApiController
    {
        BlogRepository blogRepository = new BlogRepository(StorageConfig.TableClient);

        // GET api/blog/getBlog
        [Route("getBlog")]
        public Blog GetBlog(DateTime date, string title)
        {
            var blog = blogRepository.FindBlog(date, title);
            return blog == null ? null : new Blog()
            {
                Date = blog.DateString,
                Title = blog.Title,
                Tags = blog.Tags,
                Content = blog.Content,
                ReadersCount = blog.ReadersCount,
                CommentsCount = blog.CommentsCount
            };
        }
    }
}
