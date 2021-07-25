using System.Collections.Generic;
using System.Threading.Tasks;
using SmokeEnGrill.API.Dtos;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Data
{
    public interface IStockRepository
    {
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void DeleteAll<T>(List<T> entities) where T : class;
        Task<bool> SaveAll();
        // Task<IEnumerable<Tablet>> MaintainairTablets(int maintenairId);
        // Task<IEnumerable<Tablet>> StoreTablets(int storeId);
        // Task<List<Tablet>> GetTabletByImei(string imei);
        // Task StockAllocation(int insertUserId,int tabletTypeId, StockAllocationDto stockAllocationDto);
        // void TabletAllocation(int insertUserId, StockAllocationDto stockAllocationDto);
    }
}