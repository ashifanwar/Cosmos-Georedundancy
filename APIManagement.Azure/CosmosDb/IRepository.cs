using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace APIManagement.Azure.CosmosDb
{
    public interface IRepository<T> where T: class
    {
        Task<T> CreateItemOnEastAsync(T item);

        Task<T> CreateItemOnWestAsync(T item);

        Task<T> CreateItemAsync(T item);

        Task<IEnumerable<T>> GetAllItem();

        Task<T> GetItemByIdEastAsync(string id);

        Task<T> GetItemByIdWestAsync(string id);

        Task<T> GetItemByIdAsync(string id);

        Task<T> Update(string id, T item);
    }
}
