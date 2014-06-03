using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MoeAtHome.Repositories
{
    public class Repository<T> : IRepository<T> where T : TableEntity, new()
    {
        private CloudTable table = null;
        protected CloudTable Table
        {
            get { return table; }
        }

        public Repository(CloudTable table)
        {
            this.table = table;
            table.CreateIfNotExists();
        }

        public Task<TableResult> AddAsync(T entity)
        {
            var operation = TableOperation.Insert(entity);

            return table.ExecuteAsync(operation);
        }

        public Task<TableResult> UpdateAsync(T entity)
        {
            var operation = TableOperation.Replace(entity);

            return table.ExecuteAsync(operation);
        }

        public Task<TableResult> RemoveAsync(T entity)
        {
            var operation = TableOperation.Delete(entity);

            return table.ExecuteAsync(operation);
        }

        public async Task<T> FindAsync(string partitionKey, string rowKey)
        {
            var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            return (await table.ExecuteAsync(operation)).Result as T;
        }

        public IQueryable<T> Query()
        {
            return table.CreateQuery<T>();
        }
    }
}