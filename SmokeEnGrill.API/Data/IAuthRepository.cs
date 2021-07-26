using System.Threading.Tasks;
using SmokeEnGrill.API.Dtos;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Data
{
    public interface IAuthRepository
    {
        Task<bool> SaveAll();
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
        Task<User> GetUserById(int id);
        Task<bool> SendEmail(EmailFormDto emailFormDto);
        Task<User> GetUser(int id, bool isCurrentUser);
        Task<User> GetUserByEmail(string email);
        Task<bool> SendResetPasswordLink(string email, string code);
        Task<bool> UserNameExist(string userName);
                Task<User> GetUserByCode(string code);









    }
}