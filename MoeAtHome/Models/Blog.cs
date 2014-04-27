using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoeAtHome.Models
{
    /// <summary>
    /// 博文
    /// </summary>
    public class Blog : TableEntity
    {
        public const string TableName = "Blog";
        public const string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return RowKey; }
            set { RowKey = value; }
        }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date
        {
            get { return DateTime.Parse(PartitionKey); }
            set { PartitionKey = value.ToString(DateFormat); }
        }

        [IgnoreProperty]
        public string DateString
        {
            get { return PartitionKey; }
        }

        /// <summary>
        /// 标签
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 阅读数
        /// </summary>
        public uint ReadersCount { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        public uint CommentsCount { get; set; }
    }
}