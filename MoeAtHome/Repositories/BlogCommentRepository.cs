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

        public async Task<IQueryable<BlogComment>> QueryBlogCommentsDescendingAsync(
            BlogKey key, long startRowTick, int count)
        {
            var keySerial = key.Serialize();
            var startRowTickSerial = startRowTick.ToString("D19");
            var startComment = await FindAsync(keySerial, startRowTickSerial);
            //没找到开始数据
            //从头查找
            if (startComment == null)
            {
                return (from c in Table.CreateQuery<BlogComment>()
                        where c.PartitionKey == key.Serialize()
                        select c).Take(count);
            }

            //RowKey递增排序
            return (from c in Table.CreateQuery<BlogComment>()
                    where c.PartitionKey == key.Serialize() && c.RowKey.CompareTo(startRowTickSerial) > 0
                    select c).Take(count);
        }
    }
}