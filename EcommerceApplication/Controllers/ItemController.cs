using EcommerceApplication.Application.Services;
using EcommerceApplication.Data;
using EcommerceApplication.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ItemController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICartService _cartService;
        public ItemController(AppDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateItem(string userid, ItemData item)
        {
            try
            {
                var newItem = new ItemData
                {
                    Name = item.Name,
                    Amount = item.Amount,
                    AddToCart = item.AddToCart,
                    Description = item.Description
                };
                await _context.ItemDatas.AddAsync(newItem);
                await _context.SaveChangesAsync();
                if (item.AddToCart)
                {
                    var newCart = new CartItem
                    {
                        ItemDataId = newItem.Id,
                        DateAdded = DateTime.Now,
                        Amount = newItem.Amount,
                        CreatedBy = userid
                    };
                    await _cartService.AddToCartAsync(newItem.Id, userid, newCart);
                }
                return Ok( new { newItem });
            }
            catch (Exception ex)
            {
                return new JsonResult("Something went wrong") { StatusCode = 500 };
            }
        }
    }
}
