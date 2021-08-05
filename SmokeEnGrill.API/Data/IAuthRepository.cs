using System.Collections.Generic;
using System.Threading.Tasks;
using EducNotes.API.Dtos;
using SmokeEnGrill.API.Dtos;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Data
{
    public interface IAuthRepository
    {
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void DeleteAll<T>(List<T> entities) where T : class;
        Task<bool> SaveAll();
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
        Task<User> GetUserById(int id);
        Task<User> GetUser(int id, bool isCurrentUser);
        Task<User> GetUserByEmail(string email);
        Task<bool> UserNameExist(string userName, int currentUserId);
        Task<User> GetUserByEmailAndLogin(string username, string email);
        Task<bool> EmailExist(string email);

    }
}