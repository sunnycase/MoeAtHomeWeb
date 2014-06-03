using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoeAtHome.Repositories
{
    /// <summary>
    /// 仓储接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IRepository<T> where T : TableEntity
    {
        Task<TableResult> AddAsync(T entity);
        Task<TableResult> UpdateAsync(T entity);
        Task<TableResult> RemoveAsync(T entity);
        Task<T> FindAsync(string partitionKey, string rowKey);

        IQueryable<T> Query();
    }
}
