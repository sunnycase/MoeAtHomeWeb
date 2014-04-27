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
        TableResult Add(T entity);
        Task<TableResult> AddAsync(T entity);
        TableResult Update(T entity);
        Task<TableResult> UpdateAsync(T entity);
        TableResult Remove(T entity);
        Task<TableResult> RemoveAsync(T entity);
        T Find(string partitionKey, string rowKey);
        Task<T> FindAsync(string partitionKey, string rowKey);
    }
}
