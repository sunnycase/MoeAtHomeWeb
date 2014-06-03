using Microsoft.WindowsAzure.Storage.Table;
using MoeAtHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MoeAtHome.Repositories
{
    public class BlogAmountRepository : Repository<BlogAmount>, IBlogAmountRepository
    {
        public BlogAmountRepository(CloudTableClient client)
            : base(client.GetTableReference(BlogAmount.TableName))
        {
        }

        public async Task<int> GetAmountAsync(DateTime date)
        {
            var amount = await this.FindAsync(date.ToString(BlogAmount.DateFormat), BlogAmount.TableName);
            if (amount != null)
            {
                return amount.Amount;
            }
            return 0;
        }

        public async Task AddAmountAsync(DateTime date)
        {
            var amount = await this.FindAsync(date.ToString(BlogAmount.DateFormat), BlogAmount.TableName);
            if (amount != null)
            {
                amount.Amount++;
                await this.UpdateAsync(amount);
            }
            else
            {
                await AddAsync(new BlogAmount
                    {
                        Date = date,
                        Amount = 0
                    });
            }
        }
    }
}