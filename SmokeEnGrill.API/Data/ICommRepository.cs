using System.Collections.Generic;
using System.Threading.Tasks;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Data
{
    public interface ICommRepository
    {
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void DeleteAll<T>(List<T> entities) where T : class;
        Task<bool> SaveAll();
        Task<bool> SendResetPasswordLink(User user, string token);
        Task<bool> SendEmployeeConfirmEmail(User user);
    }
}