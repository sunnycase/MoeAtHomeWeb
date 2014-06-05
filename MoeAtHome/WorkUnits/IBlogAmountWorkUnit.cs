using MoeAtHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoeAtHome.WorkUnits
{
    public interface IBlogAmountWorkUnit
    {
        Task<int> GetAmountAsync(DateTime date);
        Task AddAmountAsync(DateTime date);
        Task<IEnumerable<BlogAmount>> QueryAllAmountsDesendingAsync();
    }
}
