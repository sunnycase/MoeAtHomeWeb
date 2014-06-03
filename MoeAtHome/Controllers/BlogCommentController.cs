using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MoeAtHome.Models;
using MoeAtHome.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MoeAtHome.Controllers
{
    [RoutePrefix("api/blog/comments")]
    public class BlogCommentController : ApiController
    {
        BlogCommentRepository blogCommentRepository = new BlogCommentRepository(StorageConfig.TableClient);
        
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

        //GET api/blog/comments/{date}/{title}/{pageIndex}?pageSize=15
        [Route("{date}/{title}/{pageIndex}")]
        [HttpGet]
        public IEnumerable<ViewModels.Comment> QueryComments(DateTime date, [Required]string title,
            int pageIndex, int pageSize = 15)
        {
            return from c in blogCommentRepository.QueryBlogCommentsDescending(new BlogKey
                {
                    DateTime = date,
                    Title = title
                }).Skip(pageIndex * pageSize).Take(pageSize)
                select new ViewModels.Comment
                {
                    AuthorName = c.Author,
                    Content = c.Content,
                    DateTime = c.DateTime,
                    NestedComments = ReadNestedComments(c.NestedComments)
                };
        }

        private static List<ViewModels.Comment> ReadNestedComments(IEnumerable<NestedComment> comments)
        {
            var results = new List<ViewModels.Comment>();

            ReadNestedCommentsCore(comments, results);
            return results;
        }

        private static void ReadNestedCommentsCore(IEnumerable<NestedComment> comments, List<ViewModels.Comment> results)
        {
            foreach (var c in comments)
            {
                results.Add(new ViewModels.Comment
                    {
                        AuthorName = c.Author,
                        Content = c.Content,
                        DateTime = c.DateTime,
                        NestedComments = ReadNestedComments(c.NestedComments)
                    });
            }
        }
    }
}