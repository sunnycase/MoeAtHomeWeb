using Microsoft.WindowsAzure.Storage.Table;
using MoeAtHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MoeAtHome.Repositories
{
    public class BlogCommentRepository : Repository<BlogComment>, IBlogCommentRepository
    {
        public BlogCommentRepository(CloudTableClient client)
            : base(client.GetTableReference(BlogComment.TableName))
        {
        }

        public async Task<IEnumerable<BlogComment>> QueryBlogCommentsDescendingAsync(BlogKey key)
        {
            return (from c in Table.CreateQuery<BlogComment>()
                   where c.PartitionKey == key.Serialize()
                   select c);
        }
    }
}