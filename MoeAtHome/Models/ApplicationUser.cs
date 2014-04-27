using leeksnet.AspNet.Identity.TableStorage;
using System.Globalization;

namespace MoeAtHome.Models
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// 邮件地址
        /// </summary>
        public string Email { get; set; }

        internal static string GetPartitionKeyFromId(string id)
        {
            return id;
        }
    }
}
