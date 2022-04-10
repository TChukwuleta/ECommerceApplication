using EcommerceApplication.Application.Services;
using EcommerceApplication.Data;
using EcommerceApplication.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICartService _cartService;

        public CartController(AppDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateCart(int ItemId, CartItem cartItem, string UserId)
        {
            /*var validateUser = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == UserId);
            if(validateUser != null)
            {
                return new JsonResult("Invalid user details.") { StatusCode = 400 };
            }*/
            if (ModelState.IsValid)
            {
                var newCart = _cartService.AddToCartAsync(ItemId, UserId, cartItem);
                return Ok("Welldone");
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }
    }
}
