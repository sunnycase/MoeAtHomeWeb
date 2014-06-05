using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MoeAtHome.Models;
using MoeAtHome.WorkUnits;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MoeAtHome.Controllers
{
    [RoutePrefix("api/blogs/comments")]
    public class BlogCommentController : ApiController
    {
        IBlogCommentWorkUnit blogCommentWorkUnit = new BlogCommentWorkUnit(StorageConfig.TableClient);
        
        public BlogCommentController()
            : this(Startup.UserManagerFactory(), Startup.OAuthOptions.AccessTokenFormat)
        {
        }

        public BlogCommentController(UserManager<ApplicationUser> userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        //GET api/blogs/comments/{date}/{title}/{lastTick}?pageSize=15
        [Route("{date}/{title}/{lastTick}")]
        [HttpGet]
        public async Task<IEnumerable<ViewModels.Comment>> QueryComments(DateTime date,
            [Required]string title, long lastTick, int pageSize = 15)
        {
            return await blogCommentWorkUnit.QueryBlogCommentsDescendingAsync(new BlogKey
                {
                    DateTime = date,
                    Title = title
                }, lastTick, pageSize);
        }

        //POST api/blogs/comments/{date}/{title}
        [Route("{date}/{title}")]
        [HttpPost]
        public async Task<IHttpActionResult> PostComment(DateTime date,
            [Required]string title, [FromBody] ViewModels.PostBlogCommentViewModel model)
        {
            var author = User.Identity.IsAuthenticated ? User.Identity.Name : null;
            await blogCommentWorkUnit.PostBlogCommentAsync(new BlogKey
                {
                    DateTime = date,
                    Title = title,
                }, author, DateTime.UtcNow, model.Content);

            return Ok();
        }
    }
}