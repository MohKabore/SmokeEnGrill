using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.data;
using EducNotes.API.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Data
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ICacheRepository _cache;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContext;
        private int broadcastTokenTypeId;
        public AdminRepository(DataContext context, IMapper mapper, UserManager<User> userManager,
            ICacheRepository cache, IConfiguration config, IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _httpContext = httpContext;
            _mapper = mapper;
            _context = context;
            _cache = cache;
            broadcastTokenTypeId = _config.GetValue<int>("AppSettings:broadcastTokenTypeId");
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public async void AddAsync<T>(T entity) where T : class
        {
            await _context.AddAsync(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public void DeleteAll<T>(List<T> entities) where T : class
        {
            _context.RemoveRange(entities);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<Token>> GetTokens()
        {
        var tokens = await _cache.GetTokens();
        return tokens;
        }

        public async Task<List<Token>> GetBroadcastTokens()
        {
        List<Token> tokensCached = await _cache.GetTokens();

        var tokens = tokensCached
            .Where(t => t.TokenTypeId == broadcastTokenTypeId)
            .OrderBy(t => t.Name).ToList();
        return tokens;
        }

        public async Task<IEnumerable<Setting>> GetSettings()
        {
        var settings = await _cache.GetSettings();
        return settings;
        }

        public string ReplaceTokens(List<TokenDto> tokens, string content)
        {
        foreach (var token in tokens)
        {
            content = content.Replace(token.TokenString, token.Value);
        }
        return content;
        }

        public async Task<IEnumerable<City>> GetCities()
        {
        return (await _context.Cities.OrderBy(c => c.Name).ToListAsync());
        }

        public async Task<IEnumerable<District>> GetGetDistrictsByCityIdCities(int id)
        {
        return (await _context.Districts.Where(c => c.CityId == id).OrderBy(c => c.Name).ToListAsync());
        }

        public string GetAppSubDomain()
        {
        string subdomain = "";
        //To get subdomain
        string[] fullAddress = _httpContext.HttpContext?.Request?.Headers?["Host"].ToString()?.Split('.');
        if (fullAddress != null)
        {
            subdomain = fullAddress[0].ToLower();
            if (subdomain == "localhost:5000" || subdomain == "www" || subdomain == "educnotes")
            {
            subdomain = "";
            }
        }

        return subdomain;
        }
    }
}