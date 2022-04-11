using EcommerceApplication.Application.Services;
using EcommerceApplication.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder(int ItemId, string userid)
        {
            try
            {
                /*var validateUser = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == UserId);
                if(validateUser != null)
                {
                    return new JsonResult("Invalid user details.") { StatusCode = 400 };
                }*/
                var newOrder = await _orderService.AddOrderAsync(ItemId, userid);
                return Ok(newOrder);
            }
            catch (Exception ex)
            {
                return new JsonResult("Something went wrong") { StatusCode = 500 };
            }
        }
    }
}
