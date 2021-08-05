using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SmokeEnGrill.API.Data;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IAdminRepository _repo;
        private DataContext _context;
        public ICacheRepository _cache { get; }

        public AdminController(IConfiguration config, IMapper mapper, IAdminRepository repo,
            UserManager<User> userManager, DataContext context, ICacheRepository cache)
        {
            _cache = cache;
            _context = context;
            _repo = repo;
            _config = config;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("GetCities")]
        public async Task<IActionResult> GetAllCities()
        {
        return Ok(await _repo.GetCities());
        }

        [HttpGet("GetDistrictsByCityId/{id}")]
        public async Task<IActionResult> GetAllGetDistrictsByCityIdCities(int id)
        {
        return Ok(await _repo.GetGetDistrictsByCityIdCities(id));
        }

    }
}