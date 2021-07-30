using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmokeEnGrill.API.Dtos;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Data
{
    public class StockRepository : IStockRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public StockRepository(DataContext context, IConfiguration config, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _config = config;
            _mapper = mapper;
            _config = config;
            _userManager = userManager;
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

        public async Task<List<FoodMenuProductDto>> StoreProduct(int storeId)
        {
            var storeProducts = await _context.StoreProducts.Include(a => a.Product).Where(a => a.StoreId == storeId).ToListAsync();
            if (storeProducts.Count() > 0)
            {
                var listToReturn = new List<FoodMenuProductDto>();
                foreach (var prod in storeProducts)
                {
                    listToReturn.Add(
                        new FoodMenuProductDto
                        {
                            ProductId = prod.ProductId,
                            Qty = prod.Qty,
                            PendingQty = prod.PendingQty
                        }
                    );
                }
                return listToReturn;
            }
            return null;
        }

        public async Task<bool> SaveStockMvt(StockMvtDto newStockMvtDto)
        {
            var stockmvt = _mapper.Map<StockMvt>(newStockMvtDto);
            Add(stockmvt);

            if (newStockMvtDto.FromStoreId != null)
            {
                var fromStoreProd = await _context.StoreProducts.Where(a => a.StoreId == Convert.ToInt32(newStockMvtDto.FromStoreId)).ToListAsync();
                foreach (var prod in newStockMvtDto.Products)
                {
                    var sprods = fromStoreProd.FirstOrDefault(a => a.ProductId == prod.ProductId);
                    if (sprods != null)
                    {

                        //enregistrement InventOp
                        var inventOp = new InventOp
                        {
                            ProductId = prod.ProductId,
                            OldQty = sprods.Qty,
                            Qty = -prod.Qty,
                            FromStoreId = newStockMvtDto.FromStoreId,
                            ToStoreId = newStockMvtDto.ToStoreId,
                            Delta = sprods.Qty - prod.Qty,
                            OpDate = newStockMvtDto.MvtDate,
                            FormNum = newStockMvtDto.RefNum
                        };
                        Add(inventOp);

                        //enregistrement InventOpStockMvt
                        var stkMvtInventOp = new StockMvtInventOp
                        {
                            InventOpId = inventOp.Id,
                            StockMvtId = stockmvt.Id
                        };
                        Add(stkMvtInventOp);



                        //mise a jour du stock
                        sprods.Qty = sprods.Qty - prod.Qty;
                        Update(sprods);
                    }
                }

            }

            var tostoreProd = await _context.StoreProducts.Where(a => a.StoreId == Convert.ToInt32(newStockMvtDto.ToStoreId)).ToListAsync();
            foreach (var prod in newStockMvtDto.Products)
            {
                var sprods = tostoreProd.FirstOrDefault(a => a.ProductId == prod.ProductId);
                decimal old = 0;
                if (sprods != null)
                    old = sprods.Qty;

                //enregistrement InventOp
                var inventOp = new InventOp
                {
                    ProductId = prod.ProductId,
                    OldQty = old,
                    Qty = prod.Qty,
                    FromStoreId = newStockMvtDto.FromStoreId,
                    ToStoreId = newStockMvtDto.ToStoreId,
                    Delta = old + prod.Qty,
                    OpDate = newStockMvtDto.MvtDate,
                    FormNum = newStockMvtDto.RefNum
                };
                Add(inventOp);

                //enregistrement InventOpStockMvt
                var stkMvtInventOp = new StockMvtInventOp
                {
                    InventOpId = inventOp.Id,
                    StockMvtId = stockmvt.Id
                };
                Add(stkMvtInventOp);
                //mise a jour du stock
                sprods.Qty = sprods.Qty + prod.Qty;
                Update(sprods);

            }

            if (await SaveAll())
                return true;

            else
                return false;



        }
    }
}