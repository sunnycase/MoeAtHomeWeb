using MoeAtHome.ViewModels;
using MoeAtHome.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MoeAtHome.Models;
using Microsoft.Owin.Security;

namespace MoeAtHome.Controllers
{
    [RoutePrefix("api/blog")]
    public class BlogController : ApiController
    {
        BlogRepository blogRepository = new BlogRepository(StorageConfig.TableClient);
        BlogCommentRepository blogCommentRepository = new BlogCommentRepository(StorageConfig.TableClient);

        public BlogController()
            : this(Startup.UserManagerFactory(), Startup.OAuthOptions.AccessTokenFormat)
        {
        }

        public BlogController(UserManager<ApplicationUser> userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/blog/getBlog
        [Route("getBlog")]
        public ViewModels.Blog GetBlog(DateTime date, string title)
        {
            var blog = blogRepository.FindBlog(date, title);
            return blog == null ? null : new ViewModels.Blog()
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
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        public async Task<IHttpActionResult> PostBlog(PostBlogViewModel model)
        {
            if (!await UserManager.IsInRoleAsync(User.Identity.Name, ApplicationUser.AdministratorRoleName))
            {
                return Unauthorized();
            }
            model.Tags = (from t in model.Tags
                          where !string.IsNullOrEmpty(t)
                          select t.Trim()).Distinct().ToList();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var date = DateTime.Now;
            if (await blogRepository.FindBlogAsync(date, model.Title) != null)
            {
                ModelState.AddModelError("Title", "今天已经发过同样标题的文章了哦。");
                return BadRequest(ModelState);
            }

            await blogRepository.AddAsync(new Models.Blog()
            {
                DateTime = date,
                DateString = date.ToString(Models.Blog.DateFormat),
                Title = model.Title,
                Tags = model.Tags,
                Content = model.Content
            });
            return Ok();
        }

        [Route("queryRecentBlogs")]
        [HttpGet]
        public IEnumerable<ViewModels.Blog> QueryRecentBlogs()
        {
            return blogRepository.QueryBlogsDescending(10).Select(o => new ViewModels.Blog
                {
                    Date = o.DateString,
                    Title = o.Title,
                    Tags = o.Tags,
                    Summary = o.Content.Substring(0, Math.Min(o.Content.Length, 200)),
                    ReadersCount = o.ReadersCount,
                    CommentsCount = o.CommentsCount
                });
        }

        [Route("queryComments")]
        [HttpGet]
        public IEnumerable<ViewModels.Comment> QueryComments(BlogKey blogKey, int pageIndex, int pageSize)
        {
            return blogCommentRepository.QueryBlogCommentsDescending(blogKey, 0).
                Select(o => new ViewModels.Comment
                {
                });
        }
    }
}
