using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoeAtHome.Web.Models
{
    public class Lyric
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
        public string FileName { get; set; }
        public ulong AccessTimes { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
