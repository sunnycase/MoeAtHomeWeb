using Microsoft.WindowsAzure.Storage.Table;
using MoeAtHome.Models;
using MoeAtHome.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MoeAtHome.WorkUnits
{
    public class BlogAmountWorkUnit : IBlogAmountWorkUnit
    {
        IRepository<BlogAmount> blogRepo;

        public BlogAmountWorkUnit(CloudTableClient client)
        {
            blogRepo = new Repository<BlogAmount>(client.GetTableReference(BlogAmount.TableName));
        }

        public async Task<int> GetAmountAsync(DateTime date)
        {
            var amount = await blogRepo.FindAsync(date.ToString(BlogAmount.DateFormat), BlogAmount.TableName);
            if (amount != null)
            {
                return amount.Amount;
            }
            return 0;
        }

        public async Task AddAmountAsync(DateTime date)
        {
            var amount = await blogRepo.FindAsync(date.ToString(BlogAmount.DateFormat), BlogAmount.TableName);
            if (amount != null)
            {
                amount.Amount++;
                await blogRepo.UpdateAsync(amount);
            }
            else
            {
                await blogRepo.AddAsync(new BlogAmount
                    {
                        Date = date,
                        Amount = 0
                    });
            }
        }

        public async Task<IEnumerable<BlogAmount>> QueryAllAmountsDesendingAsync()
        {
            return from a in blogRepo.Query().AsEnumerable()
                   orderby a.Date descending
                   select a;
        }
    }
}