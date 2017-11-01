using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoeAtHome.Web.Infrastructure;

namespace MoeAtHome.Web.Repository
{
    public interface IFriendRepository
    {
        Task<IReadOnlyList<Friend>> GetAllAsync();
    }
}
