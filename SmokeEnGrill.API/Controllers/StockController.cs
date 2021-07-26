using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmokeEnGrill.API.Data;
using SmokeEnGrill.API.Dtos;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepo;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public StockController(IStockRepository stockRepo, DataContext context, IMapper mapper, UserManager<User> userManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _stockRepo = stockRepo;
        }

        [HttpGet("StoreProduct/{storeId}")]
        public async Task<IActionResult> StoreProduct(int storeId)
        {
            var productFromRepo = await _stockRepo.StoreProduct(storeId);
            if (productFromRepo != null || productFromRepo.Count() > 0)
                return Ok(productFromRepo);

            return NotFound();
        }








        [HttpPost("AddStore")]
        public async Task<IActionResult> AddStore(NewStoreDto model)
        {
            var store = _mapper.Map<Store>(model);
            _stockRepo.Add(store);
            if (await _stockRepo.SaveAll())
                return Ok(store);

            return BadRequest();
        }


        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct(NewProductDto model)
        {
            var product = _mapper.Map<Product>(model);
            _stockRepo.Add(product);
            if (await _stockRepo.SaveAll())
                return Ok();

            return BadRequest("failed to create the product");
        }


        [HttpPost("CreateMenu")]
        public async Task<IActionResult> CreateMenu(NewMenuDto model)
        {
            var menuToCreate = new Menu
            {
                Status = 1,
                Name = model.Name
            };

            _stockRepo.Add(menuToCreate);

            foreach (var prod in model.Products)
            {
                var newMenuProduct = new MenuProduct
                {
                    MenuId = menuToCreate.Id,
                    Qty = prod.Qty,
                    ProductId = prod.ProductId
                };
                _stockRepo.Add(newMenuProduct);
            }

            if (await _stockRepo.SaveAll())
                return Ok();

            return BadRequest("failed to create the Menu");
        }

        [HttpPut("DelMenu/{menuId}")]
        public async Task<IActionResult> DelMenu(int menuId)
        {
            var menu = _context.Menus.FirstOrDefaultAsync(a => a.Id == menuId);
            if (menu != null)
            {
                _stockRepo.Delete(menu);
                if (await _stockRepo.SaveAll())
                    return Ok();

                return BadRequest("failed to delete the product");
            }
            return NotFound();
        }

        [HttpPost("StockMvt")]
        public async Task<IActionResult> StockMvt(NewStockMvtDto model)
        {
            model.InsertUserId = Convert.ToInt32(_userManager.GetUserId(User));
            if(await _stockRepo.SaveStockMvt(model))
            return Ok();

            return BadRequest();
        }

    }
}