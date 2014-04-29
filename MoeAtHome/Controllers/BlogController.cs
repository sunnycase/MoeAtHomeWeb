using MoeAtHome.ViewModels;
using MoeAtHome.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

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

        // POST api/blog/postBlog
        [Route("postBlog")]
        [Authorize(Roles = Models.ApplicationUser.AdministratorRoleName)]
        public async Task<IHttpActionResult> PostBlog(PostBlogViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var date = DateTime.Today;
            if (await blogRepository.FindBlogAsync(date, model.Title) != null)
            {
                ModelState.AddModelError("Title", "今天已经发过同样标题的文章了哦。");
                return BadRequest(ModelState);
            }

            await blogRepository.AddAsync(new Models.Blog()
            {
                Date = date,
                Title = model.Title,
                Content = model.Content
            });
            return Ok();
        }
    }
}
