using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoeAtHome.Web.Infrastructure;
using MongoDB.Driver;

namespace MoeAtHome.Web.Repository
{
    internal class FriendRepository : IFriendRepository
    {
        private readonly IMongoCollection<Friend> _friendRepo;

        public FriendRepository(AppDbContext dbContext)
        {
            _friendRepo = dbContext.Friends;
        }

        public async Task<IReadOnlyList<Friend>> GetAllAsync()
        {
            var options = new FindOptions<Friend>
            {
                Sort = new SortDefinitionBuilder<Friend>().Ascending(o => o.Name)
            };
            return await (await _friendRepo.FindAsync(FilterDefinition<Friend>.Empty, options)).ToListAsync();
        }
    }
}
