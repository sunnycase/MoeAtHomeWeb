using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoeAtHome.Web.Infrastructure;

namespace MoeAtHome.Web.Models
{
    internal class Profile : AutoMapper.Profile
    {
        public Profile()
        {
            CreateMap<Friend, FriendViewModel>(AutoMapper.MemberList.Destination);
        }
    }
}
