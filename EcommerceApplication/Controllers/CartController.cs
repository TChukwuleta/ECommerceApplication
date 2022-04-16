using EcommerceApplication.Application.Services;
using EcommerceApplication.Data;
using EcommerceApplication.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string accessToken;

        public CartController(AppDbContext context, ICartService cartService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            if (accessToken == null)
            {
                throw new Exception("You are not authorized!");
            }
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateCart(int ItemId, CartItem cartItem, string UserId)
        {
            try
            {
                /*var validateUser = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == UserId);
                if(validateUser != null)
                {
                    return new JsonResult("Invalid user details.") { StatusCode = 400 };
                }*/
                var newCart = await _cartService.AddToCartAsync(ItemId, UserId, cartItem);
                return Ok(newCart);
            }
            catch (Exception ex)
            {

                return new JsonResult("Something went wrong") { StatusCode = 500 };
            }
        }
    }
}
