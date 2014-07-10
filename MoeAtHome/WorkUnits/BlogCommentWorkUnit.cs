using Microsoft.WindowsAzure.Storage.Table;
using MoeAtHome.Models;
using MoeAtHome.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MoeAtHome.WorkUnits
{
    public class BlogCommentWorkUnit : IBlogCommentWorkUnit
    {
        IRepository<BlogComment> blogCommentRepo;
        public BlogCommentWorkUnit(CloudTableClient client)
        {
            blogCommentRepo = new Repository<BlogComment>(client.GetTableReference(BlogComment.TableName));
        }

        public async Task<IEnumerable<ViewModels.Comment>> QueryBlogCommentsDescendingAsync(
            BlogKey key, long startRowTick, int count)
        {
            var keySerial = key.Serialize();
            var startRowTickSerial = startRowTick.ToString("D19");
            var startComment = await blogCommentRepo.FindAsync(keySerial, startRowTickSerial);
            IEnumerable<BlogComment> comments = null;
            //没找到开始数据
            //从头查找
            if (startComment == null)
            {
                comments = (from c in blogCommentRepo.Query()
                            where c.PartitionKey == keySerial
                            select c).Take(count).AsEnumerable();
            }
            //RowKey递增排序
            else
            {
                comments = (from c in blogCommentRepo.Query()
                            where c.PartitionKey == key.Serialize() && c.RowKey.CompareTo(startRowTickSerial) > 0
                            select c).Take(count).AsEnumerable();
            }
            return from c in comments
                   let ns = from n in c.NestedComments
                            orderby n.DateTime
                            select new ViewModels.Comment
                            {
                                AuthorName = n.Author,
                                Content = n.Content,
                                DateTime = n.DateTime
                            }
                   select new ViewModels.Comment
                   {
                       AuthorName = c.Author,
                       Content = c.Content,
                       Floor = c.Floor,
                       DateTime = c.DateTime,
                       NestedComments = ns
                   };
        }

        public async Task PostBlogCommentAsync(BlogKey key, string author, DateTime dateTime, string content)
        {
            await blogCommentRepo.AddAsync(new BlogComment
                {
                    Author = author,
                    BlogKey = key,
                    DateTime = dateTime,
                    Content = content,
                    Floor = await GetAvailableFloorAsync(key)
                });
        }

        private async Task<int> GetAvailableFloorAsync(BlogKey key)
        {
            var floor = (from c in blogCommentRepo.Query()
                         where c.PartitionKey == key.Serialize()
                         select c.Floor).Take(1).SingleOrDefault();
            return floor + 1;
        }
    }
}