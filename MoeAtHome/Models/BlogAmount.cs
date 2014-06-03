using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoeAtHome.Models
{
    /// <summary>
    /// 每天Blog的数量
    /// </summary>
    public class BlogAmount : TableEntity
    {
        public const string TableName = "BlogAmount";
        public const string DateFormat = "yyyy-MM-dd";


        [IgnoreProperty]
        public DateTime Date
        {
            get { return DateTime.Parse(PartitionKey); }
            set { PartitionKey = value.ToString(DateFormat); }
        }

        /// <summary>
        /// Blog 数量
        /// </summary>
        public int Amount { get; set; }

        public BlogAmount()
        {
            RowKey = TableName;
        }
    }
}