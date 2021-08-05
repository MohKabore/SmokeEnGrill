using System.Collections.Generic;
using System.Threading.Tasks;
using EducNotes.API.Dtos;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Data
{
    public interface IAdminRepository
    {
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void DeleteAll<T>(List<T> entities) where T : class;
        Task<bool> SaveAll();
        Task<IEnumerable<Setting>> GetSettings();
        Task<List<Token>> GetTokens();
        string ReplaceTokens(List<TokenDto> tokens, string content);
        Task<List<Token>> GetBroadcastTokens();
        Task<IEnumerable<District>> GetGetDistrictsByCityIdCities(int id);
        Task<IEnumerable<City>> GetCities();
        string GetAppSubDomain();
    }
}