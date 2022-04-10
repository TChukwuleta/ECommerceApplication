using EcommerceApplication.Data;
using EcommerceApplication.Domain.DTOs.Response;
using EcommerceApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApplication.Application.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        public CartService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ResultResponse> AddToCartAsync(int ItemId, string userId, CartItem cartItem)
        {
            try
            {
                /*var checkItem = await _context.CartItems.FirstOrDefaultAsync(c => c.ItemDataId == ItemId);
                if(checkItem != null)
                {
                    *//*checkItem.Amount++;*//*
                    return ResultResponse.Failure("You already have this item in your cart. The quantity has just been increased by 1");
                }*/
                var existingCartItem = await _context.CartItems.Where(c => c.ItemDataId == ItemId).FirstOrDefaultAsync();
                if (existingCartItem != null)
                {
                    existingCartItem.ItemDataId = ItemId;
                    existingCartItem.CreatedBy = userId;
                    existingCartItem.DateAdded = DateTime.Now;
                    _context.CartItems.Update(existingCartItem);
                    await _context.SaveChangesAsync();
                }

                var newCartItem = new CartItem
                {
                    ItemDataId = ItemId,
                    CreatedBy = userId,
                    DateAdded = DateTime.Now
                };
                await _context.CartItems.AddAsync(newCartItem);
                await _context.SaveChangesAsync();

                return ResultResponse.Success("Cart item added successfully", newCartItem);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ResultResponse> GetCartItemsAsync()
        {
            var items = await _context.CartItems.ToListAsync();
            if(items == null)
            {
                return ResultResponse.Failure("No record found");
            }
            return ResultResponse.Success(items);
        }

        public async Task<ResultResponse> RemoveFromCartAsync(int itemId)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.ItemDataId == itemId);
            if(cartItem == null)
            {
                return ResultResponse.Failure("Invalid item");
            }
            _context.Remove(cartItem);
            await _context.SaveChangesAsync();

            return ResultResponse.Success("You have successfully deleted this item");
        }

        public Task<ResultResponse> UpdateCartAsync(int cartId)
        {
            throw new NotImplementedException();
        }
    }
}
