using Microsoft.WindowsAzure.Storage.Table;
using MoeAtHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoeAtHome.Repositories
{
    public class BlogCommentRepository : Repository<BlogComment>, IBlogCommentRepository
    {
        public BlogCommentRepository(CloudTableClient client)
            : base(client.GetTableReference(BlogComment.TableName))
        {
        }

        public IEnumerable<BlogComment> QueryBlogCommentsDescending(BlogKey key, int count)
        {
            var query = new TableQuery<BlogComment>().Where(TableQuery.
                GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, key.Serialize()));
            
            return Table.ExecuteQuery(query).OrderByDescending(o => o.DateTime).Take(count);
        }
    }
}