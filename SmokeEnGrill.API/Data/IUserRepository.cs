using System.Collections.Generic;
using System.Threading.Tasks;
using EducNotes.API.Dtos;

namespace SmokeEnGrill.API.Data
{
    public interface IUserRepository
    {
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void DeleteAll<T>(List<T> entities) where T : class;
        Task<bool> SaveAll();
        string GetUserIDNumber(int userId, string lastName, string firstName);
        Task<bool> EditUserAccount(UserAccountForEditDto user);
        Task<bool> AddEmployee(EmployeeForEditDto user);
    }
}