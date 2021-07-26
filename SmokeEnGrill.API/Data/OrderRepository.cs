using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Data
{
    public class OrderRepository : IOrderRepository
    {
       private readonly IStockRepository _stockRepo;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public OrderRepository(IStockRepository stockRepo, DataContext context, IMapper mapper, UserManager<User> userManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _stockRepo = stockRepo;
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
    }
}