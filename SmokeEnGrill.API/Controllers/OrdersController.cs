using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmokeEnGrill.API.Data;
using SmokeEnGrill.API.Dtos;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public OrdersController(IMapper mapper, DataContext context, IOrderRepository orderRepo)
        {
            _mapper = mapper;
            _context = context;
            _orderRepo = orderRepo;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(NewOrderDto newOrderDto)
        {
            // The order params like TVA must be set in the front
            var order = _mapper.Map<Order>(newOrderDto);
            _orderRepo.Add(order);

            foreach (var line in newOrderDto.Lines)
            {
                var orderline = _mapper.Map<OrderLine>(line);
                _orderRepo.Add(orderline);

                var menuProds = await _context.menuProducts.Where(a => a.MenuId == line.MenuId).ToListAsync();
                foreach (var menu in menuProds)
                {
                    var orderlineProd = new OrderLineProduct
                    {
                        MenuId = menu.MenuId,
                        ProductId = menu.ProductId,
                        Qty = menu.Qty
                    };

                    _orderRepo.Add(orderlineProd);
                }
            }

            if(await _orderRepo.SaveAll())
            return Ok();

            return BadRequest();


        }
    }
}