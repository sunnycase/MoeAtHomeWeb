using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
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
        /// 时间
        /// </summary>
        public DateTime DateTime { get; set; }

        [IgnoreProperty]
        public string DateString
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }

        /// <summary>
        /// 标签
        /// </summary>
        [IgnoreProperty]
        public List<string> Tags
        {
            get { return GetTags(SerializedTags); }
            set { SerializedTags = JsonConvert.SerializeObject(value); }
        }
        public string SerializedTags { get; set; }

        public static List<string> GetTags(string serialized)
        {
            return JsonConvert.DeserializeObject<List<string>>(serialized ?? string.Empty);
        }

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